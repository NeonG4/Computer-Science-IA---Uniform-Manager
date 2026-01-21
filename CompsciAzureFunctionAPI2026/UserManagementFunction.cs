using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using CompsciAzureFunctionAPI2026.Services;

namespace CompsciAzureFunctionAPI2026
{
    /// <summary>
    /// Azure Functions for User Management with role-based access control
    /// Only Administrators can view user lists
    /// </summary>
    public class UserManagementFunction
    {
        private readonly ILogger<UserManagementFunction> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuthorizationService _authService;

        public UserManagementFunction(ILogger<UserManagementFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _authService = new AuthorizationService(logger);
        }

        #region Get Users (Admin only)

        /// <summary>
        /// GET all users - Only Administrators can access this
        /// </summary>
        [Function("GetUsers")]
        public async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GetUsers function triggered.");

            try
            {
                // Get requesting user ID from query
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new UserListResponse
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

                // Check permissions - Only administrators can view user lists
                var accountLevel = await _authService.GetUserAccountLevel(connection, userId);
                if (accountLevel != 0) // 0 = Administrator
                {
                    return new UnauthorizedObjectResult(new UserListResponse
                    {
                        Success = false,
                        Message = "Only administrators can view user lists."
                    });
                }

                // Get all users (excluding passwords)
                var users = new List<UserDto>();
                await using var cmd = new SqlCommand(@"
                    SELECT UserId, Username, FirstName, LastName, Email, AccountLevel, 
                           CreatedDate, LastModified
                    FROM [dbo].[UserInfo]
                    ORDER BY LastName, FirstName", connection);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new UserDto
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Email = reader.GetString(4),
                        AccountLevel = reader.GetInt32(5)
                    });
                }

                return new OkObjectResult(new UserListResponse
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Users = users,
                    TotalCount = users.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");
                return new ObjectResult(new UserListResponse
                {
                    Success = false,
                    Message = $"Error retrieving users: {ex.Message}"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        #endregion

        #region Response Models

        public class UserListResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<UserDto>? Users { get; set; }
            public int TotalCount { get; set; }
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int AccountLevel { get; set; }
        }

        #endregion
    }
}
