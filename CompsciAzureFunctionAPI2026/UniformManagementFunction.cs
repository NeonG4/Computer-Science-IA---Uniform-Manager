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
    /// <summary>
    /// Azure Functions for Uniform Management with role-based access control
    /// </summary>
    public class UniformManagementFunction
    {
        private readonly ILogger<UniformManagementFunction> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuthorizationService _authService;

        public UniformManagementFunction(ILogger<UniformManagementFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _authService = new AuthorizationService(logger);
        }

        #region Get Uniforms (All roles can read)

        /// <summary>
        /// GET all uniforms - All authenticated users can read
        /// </summary>
        [Function("GetUniforms")]
        public async Task<IActionResult> GetUniforms(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GetUniforms function triggered.");

            try
            {
                // Get requesting user ID from query
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new UniformListResponse
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

                // Check permissions
                var accountLevel = await _authService.GetUserAccountLevel(connection, userId);
                if (!_authService.CanRead(accountLevel))
                {
                    return new UnauthorizedObjectResult(new UniformListResponse
                    {
                        Success = false,
                        Message = "You do not have permission to view uniforms."
                    });
                }

                // Get all uniforms
                var uniforms = new List<UniformDto>();
                await using var cmd = new SqlCommand(@"
                    SELECT UniformId, UniformIdentifier, UniformType, Size, 
                           IsCheckedOut, AssignedStudentId, Conditions, 
                           CreatedDate, LastModified
                    FROM [dbo].[Uniforms]
                    ORDER BY UniformIdentifier", connection);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    uniforms.Add(MapToDto(reader));
                }

                return new OkObjectResult(new UniformListResponse
                {
                    Success = true,
                    Message = "Uniforms retrieved successfully.",
                    Uniforms = uniforms,
                    TotalCount = uniforms.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uniforms.");
                return new ObjectResult(new UniformListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving uniforms."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// GET single uniform by identifier
        /// </summary>
        [Function("GetUniform")]
        public async Task<IActionResult> GetUniform(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uniforms/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation("GetUniform function triggered for ID: {Id}", id);

            try
            {
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new UniformResponse
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

                var accountLevel = await _authService.GetUserAccountLevel(connection, userId);
                if (!_authService.CanRead(accountLevel))
                {
                    return new UnauthorizedObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Unauthorized."
                    });
                }

                await using var cmd = new SqlCommand(@"
                    SELECT UniformId, UniformIdentifier, UniformType, Size, 
                           IsCheckedOut, AssignedStudentId, Conditions, 
                           CreatedDate, LastModified
                    FROM [dbo].[Uniforms]
                    WHERE UniformIdentifier = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new OkObjectResult(new UniformResponse
                    {
                        Success = true,
                        Message = "Uniform found.",
                        Uniform = MapToDto(reader)
                    });
                }

                return new NotFoundObjectResult(new UniformResponse
                {
                    Success = false,
                    Message = "Uniform not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uniform.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Create/Update/Delete Uniform (Admin only)

        /// <summary>
        /// POST create new uniform - Admin only
        /// </summary>
        [Function("CreateUniform")]
        public async Task<IActionResult> CreateUniform(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("CreateUniform function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<CreateUniformRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.UniformIdentifier))
                {
                    return new BadRequestObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Invalid request. Uniform identifier is required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check Admin permission
                var accountLevel = await _authService.GetUserAccountLevel(connection, request.RequestingUserId);
                if (!_authService.CanFullyModifyUniform(accountLevel))
                {
                    return new ForbidResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Only administrators can create uniforms."
                    }.Message);
                }

                // Check if uniform already exists
                await using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [dbo].[Uniforms] WHERE UniformIdentifier = @Id",
                    connection);
                checkCmd.Parameters.AddWithValue("@Id", request.UniformIdentifier);

                if ((int)(await checkCmd.ExecuteScalarAsync() ?? 0) > 0)
                {
                    return new ConflictObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "A uniform with this identifier already exists."
                    });
                }

                // Insert uniform
                await using var insertCmd = new SqlCommand(@"
                    INSERT INTO [dbo].[Uniforms] 
                    (UniformIdentifier, UniformType, Size, IsCheckedOut, ModifiedBy, LastModified)
                    VALUES (@Id, @Type, @Size, 0, @UserId, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

                insertCmd.Parameters.AddWithValue("@Id", request.UniformIdentifier);
                insertCmd.Parameters.AddWithValue("@Type", request.UniformType);
                insertCmd.Parameters.AddWithValue("@Size", request.Size);
                insertCmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);

                int newId = (int)(await insertCmd.ExecuteScalarAsync() ?? 0);

                _logger.LogInformation("Uniform created with ID: {Id}", newId);

                return new OkObjectResult(new UniformResponse
                {
                    Success = true,
                    Message = "Uniform created successfully.",
                    Uniform = new UniformDto
                    {
                        UniformId = newId,
                        UniformIdentifier = request.UniformIdentifier,
                        UniformType = request.UniformType,
                        UniformTypeName = ((UniformClothing)request.UniformType).ToString(),
                        Size = request.Size,
                        IsCheckedOut = false,
                        IsInGoodCondition = true
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating uniform.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// PUT update uniform details (type, size) - Admin only
        /// </summary>
        [Function("UpdateUniform")]
        public async Task<IActionResult> UpdateUniform(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequest req)
        {
            _logger.LogInformation("UpdateUniform function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<UpdateUniformRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.UniformIdentifier))
                {
                    return new BadRequestObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Invalid request."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var accountLevel = await _authService.GetUserAccountLevel(connection, request.RequestingUserId);
                if (!_authService.CanFullyModifyUniform(accountLevel))
                {
                    return new ForbidResult("Only administrators can update uniform details.");
                }

                // Build dynamic update query
                var updates = new List<string>();
                var cmd = new SqlCommand { Connection = connection };

                if (request.UniformType.HasValue)
                {
                    updates.Add("UniformType = @Type");
                    cmd.Parameters.AddWithValue("@Type", request.UniformType.Value);
                }

                if (request.Size.HasValue)
                {
                    updates.Add("Size = @Size");
                    cmd.Parameters.AddWithValue("@Size", request.Size.Value);
                }

                if (updates.Count == 0)
                {
                    return new BadRequestObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "No updates specified."
                    });
                }

                updates.Add("LastModified = GETDATE()");
                updates.Add("ModifiedBy = @UserId");

                cmd.CommandText = $@"
                    UPDATE [dbo].[Uniforms]
                    SET {string.Join(", ", updates)}
                    WHERE UniformIdentifier = @Id";

                cmd.Parameters.AddWithValue("@Id", request.UniformIdentifier);
                cmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Uniform not found."
                    });
                }

                return new OkObjectResult(new UniformResponse
                {
                    Success = true,
                    Message = "Uniform updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating uniform.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// DELETE uniform - Admin only
        /// </summary>
        [Function("DeleteUniform")]
        public async Task<IActionResult> DeleteUniform(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "uniforms/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation("DeleteUniform function triggered for: {Id}", id);

            try
            {
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "User ID is required."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var accountLevel = await _authService.GetUserAccountLevel(connection, userId);
                if (!_authService.CanFullyModifyUniform(accountLevel))
                {
                    return new ForbidResult("Only administrators can delete uniforms.");
                }

                await using var cmd = new SqlCommand(
                    "DELETE FROM [dbo].[Uniforms] WHERE UniformIdentifier = @Id",
                    connection);
                cmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Uniform not found."
                    });
                }

                return new OkObjectResult(new UniformResponse
                {
                    Success = true,
                    Message = "Uniform deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting uniform.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Check Out/In Uniform (User and Admin)

        /// <summary>
        /// POST check out or check in a uniform - User and Admin
        /// </summary>
        [Function("CheckOutUniform")]
        public async Task<IActionResult> CheckOutUniform(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uniforms/checkout")] HttpRequest req)
        {
            _logger.LogInformation("CheckOutUniform function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<CheckOutUniformRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.UniformIdentifier))
                {
                    return new BadRequestObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Invalid request."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var accountLevel = await _authService.GetUserAccountLevel(connection, request.RequestingUserId);
                if (!_authService.CanModifyCheckOutAndConditions(accountLevel))
                {
                    return new ForbidResult("You do not have permission to check out uniforms.");
                }

                // Update check out status
                await using var cmd = new SqlCommand(@"
                    UPDATE [dbo].[Uniforms]
                    SET IsCheckedOut = @CheckOut,
                        AssignedStudentId = @StudentId,
                        LastModified = GETDATE(),
                        ModifiedBy = @UserId
                    WHERE UniformIdentifier = @Id", connection);

                cmd.Parameters.AddWithValue("@CheckOut", request.CheckOut);
                cmd.Parameters.AddWithValue("@StudentId", (object?)request.StudentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);
                cmd.Parameters.AddWithValue("@Id", request.UniformIdentifier);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Uniform not found."
                    });
                }

                string action = request.CheckOut ? "checked out" : "checked in";
                return new OkObjectResult(new UniformResponse
                {
                    Success = true,
                    Message = $"Uniform {action} successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking out uniform.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Update Conditions (User and Admin)

        /// <summary>
        /// PUT update uniform conditions - User and Admin
        /// </summary>
        [Function("UpdateUniformConditions")]
        public async Task<IActionResult> UpdateConditions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "uniforms/conditions")] HttpRequest req)
        {
            _logger.LogInformation("UpdateConditions function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<UpdateConditionsRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.UniformIdentifier))
                {
                    return new BadRequestObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Invalid request."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var accountLevel = await _authService.GetUserAccountLevel(connection, request.RequestingUserId);
                if (!_authService.CanModifyCheckOutAndConditions(accountLevel))
                {
                    return new ForbidResult("You do not have permission to update uniform conditions.");
                }

                // Convert conditions to JSON
                string conditionsJson = request.Conditions.Length > 0
                    ? JsonSerializer.Serialize(request.Conditions)
                    : "[]";

                await using var cmd = new SqlCommand(@"
                    UPDATE [dbo].[Uniforms]
                    SET Conditions = @Conditions,
                        LastModified = GETDATE(),
                        ModifiedBy = @UserId
                    WHERE UniformIdentifier = @Id", connection);

                cmd.Parameters.AddWithValue("@Conditions", conditionsJson);
                cmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);
                cmd.Parameters.AddWithValue("@Id", request.UniformIdentifier);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new UniformResponse
                    {
                        Success = false,
                        Message = "Uniform not found."
                    });
                }

                return new OkObjectResult(new UniformResponse
                {
                    Success = true,
                    Message = "Uniform conditions updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating uniform conditions.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Helper Methods

        private UniformDto MapToDto(SqlDataReader reader)
        {
            var uniformType = reader.GetInt32(reader.GetOrdinal("UniformType"));
            var conditionsJson = reader.IsDBNull(reader.GetOrdinal("Conditions"))
                ? "[]"
                : reader.GetString(reader.GetOrdinal("Conditions"));

            int[] conditions = Array.Empty<int>();
            try
            {
                conditions = JsonSerializer.Deserialize<int[]>(conditionsJson) ?? Array.Empty<int>();
            }
            catch { }

            return new UniformDto
            {
                UniformId = reader.GetInt32(reader.GetOrdinal("UniformId")),
                UniformIdentifier = reader.GetString(reader.GetOrdinal("UniformIdentifier")),
                UniformType = uniformType,
                UniformTypeName = ((UniformClothing)uniformType).ToString(),
                Size = reader.GetInt32(reader.GetOrdinal("Size")),
                IsCheckedOut = reader.GetBoolean(reader.GetOrdinal("IsCheckedOut")),
                AssignedStudentId = reader.IsDBNull(reader.GetOrdinal("AssignedStudentId"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("AssignedStudentId")),
                Conditions = conditions,
                ConditionNames = conditions.Select(c => ((Condition)c).ToString()).ToArray(),
                IsInGoodCondition = conditions.Length == 0,
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                LastModified = reader.IsDBNull(reader.GetOrdinal("LastModified"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("LastModified"))
            };
        }

        #endregion
    }
}
