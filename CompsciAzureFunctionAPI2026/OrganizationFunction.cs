using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using CompsciAzureFunctionAPI2026.Models;

namespace CompsciAzureFunctionAPI2026
{
    public class OrganizationFunction
    {
        private readonly ILogger<OrganizationFunction> _logger;
        private readonly IConfiguration _configuration;

        public OrganizationFunction(ILogger<OrganizationFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// GET all organizations for a user
        /// </summary>
        [Function("GetOrganizations")]
        public async Task<IActionResult> GetOrganizations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GetOrganizations function triggered.");

            try
            {
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new GetOrganizationsResponse
                    {
                        Success = false,
                        Message = "User ID is required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Database connection string not found.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Get organizations user belongs to with their role in each
                await using var cmd = new SqlCommand(@"
                    SELECT o.OrganizationId, o.OrganizationName, o.OrganizationCode, 
                           o.Description, uo.AccountLevel
                    FROM [dbo].[Organizations] o
                    INNER JOIN [dbo].[UserOrganizations] uo 
                        ON o.OrganizationId = uo.OrganizationId
                    WHERE uo.UserId = @UserId AND o.IsActive = 1 AND uo.IsActive = 1
                    ORDER BY o.OrganizationName", connection);

                cmd.Parameters.AddWithValue("@UserId", userId);

                var organizations = new List<OrganizationDto>();
                await using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    organizations.Add(new OrganizationDto
                    {
                        OrganizationId = reader.GetInt32(0),
                        OrganizationName = reader.GetString(1),
                        OrganizationCode = reader.GetString(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        UserAccountLevel = reader.GetInt32(4)
                    });
                }

                // Always return success, even with empty list
                var response = new GetOrganizationsResponse
                {
                    Success = true,
                    Message = organizations.Count > 0 
                        ? "Organizations retrieved successfully." 
                        : "No organizations found for this user.",
                    Organizations = organizations,
                    TotalCount = organizations.Count
                };

                _logger.LogInformation("GetOrganizations completed for userId: {UserId}. Found {Count} organizations.", 
                    userId, organizations.Count);

                return new OkObjectResult(response);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error retrieving organizations.");
                return new ObjectResult(new GetOrganizationsResponse
                {
                    Success = false,
                    Message = $"Database error: {sqlEx.Message}. Make sure the Organizations and UserOrganizations tables exist."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organizations.");
                return new ObjectResult(new GetOrganizationsResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving organizations: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// POST create new organization
        /// </summary>
        [Function("CreateOrganization")]
        public async Task<IActionResult> CreateOrganization(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("CreateOrganization function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<CreateOrganizationRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.OrganizationName) ||
                    string.IsNullOrWhiteSpace(request.OrganizationCode))
                {
                    return new BadRequestObjectResult(new CreateOrganizationResponse
                    {
                        Success = false,
                        Message = "Organization name and code are required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if organization code already exists
                await using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [dbo].[Organizations] WHERE OrganizationCode = @Code",
                    connection);
                checkCmd.Parameters.AddWithValue("@Code", request.OrganizationCode);

                if ((int)(await checkCmd.ExecuteScalarAsync() ?? 0) > 0)
                {
                    return new ConflictObjectResult(new CreateOrganizationResponse
                    {
                        Success = false,
                        Message = "An organization with this code already exists."
                    });
                }

                // Begin transaction to create org and assign user as admin
                await using var transaction = connection.BeginTransaction();
                
                try
                {
                    // Create organization
                    await using var insertCmd = new SqlCommand(@"
                        INSERT INTO [dbo].[Organizations] 
                        (OrganizationName, OrganizationCode, Description, CreatedBy)
                        VALUES (@Name, @Code, @Description, @UserId);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction);

                    insertCmd.Parameters.AddWithValue("@Name", request.OrganizationName);
                    insertCmd.Parameters.AddWithValue("@Code", request.OrganizationCode);
                    insertCmd.Parameters.AddWithValue("@Description", 
                        string.IsNullOrWhiteSpace(request.Description) ? DBNull.Value : request.Description);
                    insertCmd.Parameters.AddWithValue("@UserId", request.UserId);

                    int orgId = (int)(await insertCmd.ExecuteScalarAsync() ?? 0);

                    // Add user as admin of this organization
                    await using var addUserCmd = new SqlCommand(@"
                        INSERT INTO [dbo].[UserOrganizations]
                        (UserId, OrganizationId, AccountLevel)
                        VALUES (@UserId, @OrgId, 0)", connection, transaction); // 0 = Admin

                    addUserCmd.Parameters.AddWithValue("@UserId", request.UserId);
                    addUserCmd.Parameters.AddWithValue("@OrgId", orgId);

                    await addUserCmd.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();

                    _logger.LogInformation("Organization created: {OrgName} (ID: {OrgId})", 
                        request.OrganizationName, orgId);

                    return new OkObjectResult(new CreateOrganizationResponse
                    {
                        Success = true,
                        Message = "Organization created successfully.",
                        Organization = new OrganizationDto
                        {
                            OrganizationId = orgId,
                            OrganizationName = request.OrganizationName,
                            OrganizationCode = request.OrganizationCode,
                            Description = request.Description,
                            UserAccountLevel = 0 // Creator is admin
                        }
                    });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating organization.");
                return new ObjectResult(new CreateOrganizationResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating the organization: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// POST join existing organization by code (creates pending request)
        /// </summary>
        [Function("JoinOrganization")]
        public async Task<IActionResult> JoinOrganization(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("JoinOrganization function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<JoinOrganizationRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.OrganizationCode))
                {
                    return new BadRequestObjectResult(new JoinOrganizationResponse
                    {
                        Success = false,
                        Message = "Organization code is required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Find organization by code
                await using var findOrgCmd = new SqlCommand(
                    "SELECT OrganizationId, OrganizationName FROM [dbo].[Organizations] WHERE OrganizationCode = @Code AND IsActive = 1",
                    connection);
                findOrgCmd.Parameters.AddWithValue("@Code", request.OrganizationCode.ToUpper());

                int? orgId = null;
                string? orgName = null;

                await using var reader = await findOrgCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    orgId = reader.GetInt32(0);
                    orgName = reader.GetString(1);
                }
                await reader.CloseAsync();

                if (!orgId.HasValue)
                {
                    return new NotFoundObjectResult(new JoinOrganizationResponse
                    {
                        Success = false,
                        Message = "No organization found with that code. Please check the code and try again."
                    });
                }

                // Check if user is already a member
                await using var checkMemberCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [dbo].[UserOrganizations] WHERE UserId = @UserId AND OrganizationId = @OrgId AND IsActive = 1",
                    connection);
                checkMemberCmd.Parameters.AddWithValue("@UserId", request.UserId);
                checkMemberCmd.Parameters.AddWithValue("@OrgId", orgId.Value);

                if ((int)(await checkMemberCmd.ExecuteScalarAsync() ?? 0) > 0)
                {
                    return new ConflictObjectResult(new JoinOrganizationResponse
                    {
                        Success = false,
                        Message = "You are already a member of this organization."
                    });
                }

                // Check if user already has a pending request
                await using var checkRequestCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [dbo].[OrganizationJoinRequests] WHERE UserId = @UserId AND OrganizationId = @OrgId AND Status = 0",
                    connection);
                checkRequestCmd.Parameters.AddWithValue("@UserId", request.UserId);
                checkRequestCmd.Parameters.AddWithValue("@OrgId", orgId.Value);

                if ((int)(await checkRequestCmd.ExecuteScalarAsync() ?? 0) > 0)
                {
                    return new ConflictObjectResult(new JoinOrganizationResponse
                    {
                        Success = false,
                        Message = "You already have a pending request to join this organization."
                    });
                }

                // Create join request (pending approval)
                await using var createRequestCmd = new SqlCommand(@"
                    INSERT INTO [dbo].[OrganizationJoinRequests]
                    (OrganizationId, UserId, RequestedAccountLevel, RequestMessage, Status)
                    VALUES (@OrgId, @UserId, @AccountLevel, @Message, 0)", connection);

                createRequestCmd.Parameters.AddWithValue("@OrgId", orgId.Value);
                createRequestCmd.Parameters.AddWithValue("@UserId", request.UserId);
                createRequestCmd.Parameters.AddWithValue("@AccountLevel", request.RequestedAccountLevel);
                createRequestCmd.Parameters.AddWithValue("@Message", 
                    string.IsNullOrWhiteSpace(request.RequestMessage) ? DBNull.Value : request.RequestMessage);

                await createRequestCmd.ExecuteNonQueryAsync();

                string roleText = request.RequestedAccountLevel switch
                {
                    0 => "Administrator",
                    1 => "User",
                    2 => "Viewer",
                    _ => "Member"
                };

                _logger.LogInformation("User {UserId} requested to join organization {OrgId} as {Role}", 
                    request.UserId, orgId.Value, roleText);

                return new OkObjectResult(new JoinOrganizationResponse
                {
                    Success = true,
                    Message = $"Your request to join '{orgName}' as {roleText} has been sent to the organization administrators for approval.",
                    RequiresApproval = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating join request.");
                return new ObjectResult(new JoinOrganizationResponse
                {
                    Success = false,
                    Message = $"An error occurred while creating the join request: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// GET pending join requests for an organization (Admin only)
        /// </summary>
        [Function("GetJoinRequests")]
        public async Task<IActionResult> GetJoinRequests(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GetJoinRequests function triggered.");

            try
            {
                if (!int.TryParse(req.Query["organizationId"], out int orgId))
                {
                    return new BadRequestObjectResult(new GetJoinRequestsResponse
                    {
                        Success = false,
                        Message = "Organization ID is required."
                    });
                }

                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new GetJoinRequestsResponse
                    {
                        Success = false,
                        Message = "User ID is required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if user is admin of this organization
                await using var checkAdminCmd = new SqlCommand(
                    "SELECT AccountLevel FROM [dbo].[UserOrganizations] WHERE UserId = @UserId AND OrganizationId = @OrgId AND IsActive = 1",
                    connection);
                checkAdminCmd.Parameters.AddWithValue("@UserId", userId);
                checkAdminCmd.Parameters.AddWithValue("@OrgId", orgId);

                var accountLevel = await checkAdminCmd.ExecuteScalarAsync();
                if (accountLevel == null || (int)accountLevel != 0) // 0 = Admin
                {
                    return new UnauthorizedObjectResult(new GetJoinRequestsResponse
                    {
                        Success = false,
                        Message = "Only administrators can view join requests."
                    });
                }

                // Get pending join requests
                var requests = new List<OrganizationJoinRequest>();
                await using var cmd = new SqlCommand(@"
                    SELECT jr.RequestId, jr.OrganizationId, o.OrganizationName,
                           jr.UserId, u.Username, u.Email, u.FirstName, u.LastName,
                           jr.RequestedAccountLevel, jr.Status, jr.RequestMessage,
                           jr.RequestedDate, jr.ReviewedBy, jr.ReviewedDate, jr.ReviewNotes
                    FROM [dbo].[OrganizationJoinRequests] jr
                    INNER JOIN [dbo].[Organizations] o ON jr.OrganizationId = o.OrganizationId
                    INNER JOIN [dbo].[UserInfo] u ON jr.UserId = u.UserId
                    WHERE jr.OrganizationId = @OrgId AND jr.Status = 0
                    ORDER BY jr.RequestedDate DESC", connection);

                cmd.Parameters.AddWithValue("@OrgId", orgId);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    requests.Add(new OrganizationJoinRequest
                    {
                        RequestId = reader.GetInt32(0),
                        OrganizationId = reader.GetInt32(1),
                        OrganizationName = reader.GetString(2),
                        UserId = reader.GetInt32(3),
                        UserName = $"{reader.GetString(6)} {reader.GetString(7)}", // FirstName LastName
                        UserEmail = reader.GetString(5),
                        RequestedAccountLevel = reader.GetInt32(8),
                        Status = reader.GetInt32(9),
                        RequestMessage = reader.IsDBNull(10) ? null : reader.GetString(10),
                        RequestedDate = reader.GetDateTime(11),
                        ReviewedBy = reader.IsDBNull(12) ? null : reader.GetInt32(12),
                        ReviewedDate = reader.IsDBNull(13) ? null : reader.GetDateTime(13),
                        ReviewNotes = reader.IsDBNull(14) ? null : reader.GetString(14)
                    });
                }

                return new OkObjectResult(new GetJoinRequestsResponse
                {
                    Success = true,
                    Message = "Join requests retrieved successfully.",
                    Requests = requests,
                    TotalCount = requests.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving join requests.");
                return new ObjectResult(new GetJoinRequestsResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving join requests: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// POST approve or reject a join request (Admin only)
        /// </summary>
        [Function("ReviewJoinRequest")]
        public async Task<IActionResult> ReviewJoinRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("ReviewJoinRequest function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<ReviewJoinRequestRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null)
                {
                    return new BadRequestObjectResult(new ReviewJoinRequestResponse
                    {
                        Success = false,
                        Message = "Invalid request."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Get the join request details
                await using var getRequestCmd = new SqlCommand(
                    "SELECT OrganizationId, UserId, RequestedAccountLevel, Status FROM [dbo].[OrganizationJoinRequests] WHERE RequestId = @RequestId",
                    connection);
                getRequestCmd.Parameters.AddWithValue("@RequestId", request.RequestId);

                int? orgId = null;
                int? requestUserId = null;
                int? requestedAccountLevel = null;
                int? status = null;

                await using var reader = await getRequestCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    orgId = reader.GetInt32(0);
                    requestUserId = reader.GetInt32(1);
                    requestedAccountLevel = reader.GetInt32(2);
                    status = reader.GetInt32(3);
                }
                await reader.CloseAsync();

                if (!orgId.HasValue)
                {
                    return new NotFoundObjectResult(new ReviewJoinRequestResponse
                    {
                        Success = false,
                        Message = "Join request not found."
                    });
                }

                if (status != 0) // Not pending
                {
                    return new ConflictObjectResult(new ReviewJoinRequestResponse
                    {
                        Success = false,
                        Message = "This request has already been reviewed."
                    });
                }

                // Check if reviewer is admin of the organization
                await using var checkAdminCmd = new SqlCommand(
                    "SELECT AccountLevel FROM [dbo].[UserOrganizations] WHERE UserId = @UserId AND OrganizationId = @OrgId AND IsActive = 1",
                    connection);
                checkAdminCmd.Parameters.AddWithValue("@UserId", request.ReviewerId);
                checkAdminCmd.Parameters.AddWithValue("@OrgId", orgId.Value);

                var accountLevel = await checkAdminCmd.ExecuteScalarAsync();
                if (accountLevel == null || (int)accountLevel != 0)
                {
                    return new UnauthorizedObjectResult(new ReviewJoinRequestResponse
                    {
                        Success = false,
                        Message = "Only administrators can review join requests."
                    });
                }

                // Begin transaction
                await using var transaction = connection.BeginTransaction();

                try
                {
                    // Update request status
                    await using var updateRequestCmd = new SqlCommand(@"
                        UPDATE [dbo].[OrganizationJoinRequests]
                        SET Status = @Status, ReviewedBy = @ReviewerId, ReviewedDate = GETDATE(), ReviewNotes = @Notes
                        WHERE RequestId = @RequestId", connection, transaction);

                    updateRequestCmd.Parameters.AddWithValue("@Status", request.Approved ? 1 : 2); // 1=Approved, 2=Rejected
                    updateRequestCmd.Parameters.AddWithValue("@ReviewerId", request.ReviewerId);
                    updateRequestCmd.Parameters.AddWithValue("@Notes", 
                        string.IsNullOrWhiteSpace(request.ReviewNotes) ? DBNull.Value : request.ReviewNotes);
                    updateRequestCmd.Parameters.AddWithValue("@RequestId", request.RequestId);

                    await updateRequestCmd.ExecuteNonQueryAsync();

                    // If approved, add user to organization
                    if (request.Approved)
                    {
                        await using var addUserCmd = new SqlCommand(@"
                            INSERT INTO [dbo].[UserOrganizations]
                            (UserId, OrganizationId, AccountLevel)
                            VALUES (@UserId, @OrgId, @AccountLevel)", connection, transaction);

                        addUserCmd.Parameters.AddWithValue("@UserId", requestUserId.Value);
                        addUserCmd.Parameters.AddWithValue("@OrgId", orgId.Value);
                        addUserCmd.Parameters.AddWithValue("@AccountLevel", requestedAccountLevel.Value);

                        await addUserCmd.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();

                    string message = request.Approved
                        ? "Join request approved. User has been added to the organization."
                        : "Join request rejected.";

                    _logger.LogInformation("Join request {RequestId} {Action} by user {ReviewerId}",
                        request.RequestId, request.Approved ? "approved" : "rejected", request.ReviewerId);

                    return new OkObjectResult(new ReviewJoinRequestResponse
                    {
                        Success = true,
                        Message = message
                    });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reviewing join request.");
                return new ObjectResult(new ReviewJoinRequestResponse
                {
                    Success = false,
                    Message = $"An error occurred while reviewing the join request: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
