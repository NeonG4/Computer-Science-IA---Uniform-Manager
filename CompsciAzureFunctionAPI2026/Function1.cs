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
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IConfiguration _configuration;

        public Function1(ILogger<Function1> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("CreateAccount")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("CreateAccount function triggered.");

            try
            {
                // Parse request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var accountRequest = JsonSerializer.Deserialize<CreateAccountRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (accountRequest == null)
                {
                    return new BadRequestObjectResult(new CreateAccountResponse
                    {
                        Success = false,
                        Message = "Invalid request body."
                    });
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(accountRequest.FirstName) ||
                    string.IsNullOrWhiteSpace(accountRequest.LastName) ||
                    string.IsNullOrWhiteSpace(accountRequest.Email) ||
                    string.IsNullOrWhiteSpace(accountRequest.Password) ||
                    string.IsNullOrWhiteSpace(accountRequest.Username))
                {
                    return new BadRequestObjectResult(new CreateAccountResponse
                    {
                        Success = false,
                        Message = "All fields are required."
                    });
                }

                // Validate password length
                if (accountRequest.Password.Length < 12)
                {
                    return new BadRequestObjectResult(new CreateAccountResponse
                    {
                        Success = false,
                        Message = "Password must be at least 12 characters long."
                    });
                }

                // Validate email format
                if (!accountRequest.Email.Contains("@"))
                {
                    return new BadRequestObjectResult(new CreateAccountResponse
                    {
                        Success = false,
                        Message = "Invalid email format."
                    });
                }

                // Hash password
                string hashedPassword = PasswordHasher.HashPassword(accountRequest.Password);

                // Get connection string from configuration
                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Database connection string not found.");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if email or username already exists
                await using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [dbo].[UserInfo] WHERE [Email] = @Email OR [Username] = @Username",
                    connection);
                checkCmd.Parameters.AddWithValue("@Email", accountRequest.Email);
                checkCmd.Parameters.AddWithValue("@Username", accountRequest.Username);

                int existingCount = (int)(await checkCmd.ExecuteScalarAsync() ?? 0);
                if (existingCount > 0)
                {
                    return new ConflictObjectResult(new CreateAccountResponse
                    {
                        Success = false,
                        Message = "An account with this email or username already exists."
                    });
                }

                // Insert new user (UserId is IDENTITY, so it auto-increments)
                await using var insertCmd = new SqlCommand(
                    @"INSERT INTO [dbo].[UserInfo] 
                      ([FirstName], [LastName], [Email], [HashedPassword], [AccountLevel], [Username]) 
                      VALUES (@FirstName, @LastName, @Email, @HashedPassword, @AccountLevel, @Username);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    connection);

                insertCmd.Parameters.AddWithValue("@FirstName", accountRequest.FirstName);
                insertCmd.Parameters.AddWithValue("@LastName", accountRequest.LastName);
                insertCmd.Parameters.AddWithValue("@Email", accountRequest.Email);
                insertCmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                insertCmd.Parameters.AddWithValue("@AccountLevel", 2); // Default to level 2 (User)
                insertCmd.Parameters.AddWithValue("@Username", accountRequest.Username);

                int newUserId = (int)(await insertCmd.ExecuteScalarAsync() ?? 0);

                if (newUserId > 0)
                {
                    _logger.LogInformation("Account created successfully for email: {Email}", accountRequest.Email);
                    return new OkObjectResult(new CreateAccountResponse
                    {
                        Success = true,
                        Message = "Account created successfully.",
                        UserId = newUserId
                    });
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error occurred while creating account.");
                return new ObjectResult(new CreateAccountResponse
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
                _logger.LogError(ex, "Unexpected error occurred while creating account.");
                return new ObjectResult(new CreateAccountResponse
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
