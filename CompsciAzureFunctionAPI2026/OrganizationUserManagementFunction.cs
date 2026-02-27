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
    public class OrganizationUserManagementFunction
    {
        private readonly ILogger<OrganizationUserManagementFunction> _logger;
        private readonly IConfiguration _configuration;

        public OrganizationUserManagementFunction(ILogger<OrganizationUserManagementFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// GET all users in an organization (Admin only)
        /// </summary>
        [Function("GetOrganizationUsers")]
        public async Task<IActionResult> GetOrganizationUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GetOrganizationUsers function triggered.");

            try
            {
                if (!int.TryParse(req.Query["organizationId"], out int orgId))
                {
                    return new BadRequestObjectResult(new GetOrganizationUsersResponse
                    {
                        Success = false,
                        Message = "Organization ID is required."
                    });
                }

                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new GetOrganizationUsersResponse
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
                    return new UnauthorizedObjectResult(new GetOrganizationUsersResponse
                    {
                        Success = false,
                        Message = "Only administrators can view organization users."
                    });
                }

                // Get all users in the organization
                var users = new List<OrganizationUserDto>();
                await using var cmd = new SqlCommand(@"
                    SELECT u.UserId, u.Username, u.FirstName, u.LastName, u.Email,
                           uo.AccountLevel, uo.JoinedDate, uo.IsActive
                    FROM [dbo].[UserInfo] u
                    INNER JOIN [dbo].[UserOrganizations] uo ON u.UserId = uo.UserId
                    WHERE uo.OrganizationId = @OrgId
                    ORDER BY uo.AccountLevel, u.LastName, u.FirstName", connection);

                cmd.Parameters.AddWithValue("@OrgId", orgId);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new OrganizationUserDto
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Email = reader.GetString(4),
                        AccountLevel = reader.GetInt32(5),
                        JoinedDate = reader.GetDateTime(6),
                        IsActive = reader.GetBoolean(7)
                    });
                }

                return new OkObjectResult(new GetOrganizationUsersResponse
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Users = users,
                    TotalCount = users.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization users.");
                return new ObjectResult(new GetOrganizationUsersResponse
                {
                    Success = false,
                    Message = $"An error occurred while retrieving organization users: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// PUT update user's role in organization (Admin only)
        /// </summary>
        [Function("UpdateOrganizationUserRole")]
        public async Task<IActionResult> UpdateOrganizationUserRole(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequest req)
        {
            _logger.LogInformation("UpdateOrganizationUserRole function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<UpdateUserRoleRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null)
                {
                    return new BadRequestObjectResult(new UpdateUserRoleResponse
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

                // Check if requesting user is admin
                await using var checkAdminCmd = new SqlCommand(
                    "SELECT AccountLevel FROM [dbo].[UserOrganizations] WHERE UserId = @UserId AND OrganizationId = @OrgId AND IsActive = 1",
                    connection);
                checkAdminCmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);
                checkAdminCmd.Parameters.AddWithValue("@OrgId", request.OrganizationId);

                var accountLevel = await checkAdminCmd.ExecuteScalarAsync();
                if (accountLevel == null || (int)accountLevel != 0)
                {
                    return new UnauthorizedObjectResult(new UpdateUserRoleResponse
                    {
                        Success = false,
                        Message = "Only administrators can update user roles."
                    });
                }

                // Prevent admin from changing their own role
                if (request.RequestingUserId == request.TargetUserId)
                {
                    return new BadRequestObjectResult(new UpdateUserRoleResponse
                    {
                        Success = false,
                        Message = "You cannot change your own role."
                    });
                }

                // Update the user's role
                await using var updateCmd = new SqlCommand(@"
                    UPDATE [dbo].[UserOrganizations]
                    SET AccountLevel = @NewAccountLevel
                    WHERE UserId = @TargetUserId AND OrganizationId = @OrgId",
                    connection);

                updateCmd.Parameters.AddWithValue("@NewAccountLevel", request.NewAccountLevel);
                updateCmd.Parameters.AddWithValue("@TargetUserId", request.TargetUserId);
                updateCmd.Parameters.AddWithValue("@OrgId", request.OrganizationId);

                int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new UpdateUserRoleResponse
                    {
                        Success = false,
                        Message = "User not found in this organization."
                    });
                }

                string roleText = request.NewAccountLevel switch
                {
                    0 => "Administrator",
                    1 => "User",
                    2 => "Viewer",
                    _ => "Unknown"
                };

                _logger.LogInformation("User {TargetUserId} role updated to {Role} in org {OrgId} by {RequestingUserId}",
                    request.TargetUserId, roleText, request.OrganizationId, request.RequestingUserId);

                return new OkObjectResult(new UpdateUserRoleResponse
                {
                    Success = true,
                    Message = $"User role updated to {roleText} successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role.");
                return new ObjectResult(new UpdateUserRoleResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating user role: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// DELETE remove user from organization (Admin only)
        /// </summary>
        [Function("RemoveOrganizationUser")]
        public async Task<IActionResult> RemoveOrganizationUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequest req)
        {
            _logger.LogInformation("RemoveOrganizationUser function triggered.");

            try
            {
                if (!int.TryParse(req.Query["organizationId"], out int orgId))
                {
                    return new BadRequestObjectResult(new RemoveUserResponse
                    {
                        Success = false,
                        Message = "Organization ID is required."
                    });
                }

                if (!int.TryParse(req.Query["targetUserId"], out int targetUserId))
                {
                    return new BadRequestObjectResult(new RemoveUserResponse
                    {
                        Success = false,
                        Message = "Target user ID is required."
                    });
                }

                if (!int.TryParse(req.Query["requestingUserId"], out int requestingUserId))
                {
                    return new BadRequestObjectResult(new RemoveUserResponse
                    {
                        Success = false,
                        Message = "Requesting user ID is required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if requesting user is admin
                await using var checkAdminCmd = new SqlCommand(
                    "SELECT AccountLevel FROM [dbo].[UserOrganizations] WHERE UserId = @UserId AND OrganizationId = @OrgId AND IsActive = 1",
                    connection);
                checkAdminCmd.Parameters.AddWithValue("@UserId", requestingUserId);
                checkAdminCmd.Parameters.AddWithValue("@OrgId", orgId);

                var accountLevel = await checkAdminCmd.ExecuteScalarAsync();
                if (accountLevel == null || (int)accountLevel != 0)
                {
                    return new UnauthorizedObjectResult(new RemoveUserResponse
                    {
                        Success = false,
                        Message = "Only administrators can remove users."
                    });
                }

                // Prevent admin from removing themselves
                if (requestingUserId == targetUserId)
                {
                    return new BadRequestObjectResult(new RemoveUserResponse
                    {
                        Success = false,
                        Message = "You cannot remove yourself from the organization."
                    });
                }

                // Soft delete - set IsActive to false
                await using var deleteCmd = new SqlCommand(@"
                    UPDATE [dbo].[UserOrganizations]
                    SET IsActive = 0
                    WHERE UserId = @TargetUserId AND OrganizationId = @OrgId",
                    connection);

                deleteCmd.Parameters.AddWithValue("@TargetUserId", targetUserId);
                deleteCmd.Parameters.AddWithValue("@OrgId", orgId);

                int rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new RemoveUserResponse
                    {
                        Success = false,
                        Message = "User not found in this organization."
                    });
                }

                _logger.LogInformation("User {TargetUserId} removed from org {OrgId} by {RequestingUserId}",
                    targetUserId, orgId, requestingUserId);

                return new OkObjectResult(new RemoveUserResponse
                {
                    Success = true,
                    Message = "User removed from organization successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from organization.");
                return new ObjectResult(new RemoveUserResponse
                {
                    Success = false,
                    Message = $"An error occurred while removing user: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
