using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using CompsciAzureFunctionAPI2026.Models;
using CompsciAzureFunctionAPI2026.Services;

namespace CompsciAzureFunctionAPI2026
{
    public class LoginFunction
    {
        private readonly ILogger<LoginFunction> _logger;
        private readonly IConfiguration _configuration;

        public LoginFunction(ILogger<LoginFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("Login")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("Login function triggered.");

            try
            {
                // Parse request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginRequest == null)
                {
                    return new BadRequestObjectResult(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid request body."
                    });
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return new BadRequestObjectResult(new LoginResponse
                    {
                        Success = false,
                        Message = "Username and password are required."
                    });
                }

                // Hash password
                string hashedPassword = PasswordHasher.HashPassword(loginRequest.Password);

                // Get connection string from configuration
                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Database connection string not found.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Query user from UserInfo table
                await using var cmd = new SqlCommand(
                    @"SELECT UserId, Username, FirstName, LastName, Email, AccountLevel 
                      FROM [dbo].[UserInfo] 
                      WHERE Username = @Username AND HashedPassword = @HashedPassword",
                    connection);
                
                cmd.Parameters.AddWithValue("@Username", loginRequest.Username);
                cmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);

                await using var reader = await cmd.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var userInfo = new UserInfo
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.GetString(reader.GetOrdinal("Username")).Trim(),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim(),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")).Trim(),
                        Email = reader.GetString(reader.GetOrdinal("Email")).Trim(),
                        AccountLevel = reader.GetInt32(reader.GetOrdinal("AccountLevel"))
                    };

                    _logger.LogInformation("Login successful for user: {Username}", loginRequest.Username);
                    
                    return new OkObjectResult(new LoginResponse
                    {
                        Success = true,
                        Message = "Login successful.",
                        User = userInfo
                    });
                }
                else
                {
                    _logger.LogWarning("Login failed for username: {Username}", loginRequest.Username);
                    
                    return new UnauthorizedObjectResult(new LoginResponse
                    {
                        Success = false,
                        Message = "No account available with this username and password."
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error occurred during login.");
                return new ObjectResult(new LoginResponse
                {
                    Success = false,
                    Message = "A database error occurred. Please try again later."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during login.");
                return new ObjectResult(new LoginResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
