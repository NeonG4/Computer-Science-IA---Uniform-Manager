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
                    Message = "An error occurred while creating the organization."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
