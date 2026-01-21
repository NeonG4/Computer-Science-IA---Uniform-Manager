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
    /// Azure Functions for Student Management with role-based access control
    /// Admins: Full CRUD operations
    /// Users: Read-only access
    /// Viewers: Read-only access
    /// </summary>
    public class StudentManagementFunction
    {
        private readonly ILogger<StudentManagementFunction> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuthorizationService _authService;

        public StudentManagementFunction(ILogger<StudentManagementFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _authService = new AuthorizationService(logger);
        }

        #region Get Students (All roles can read)

        /// <summary>
        /// GET all students - All authenticated users can read
        /// </summary>
        [Function("GetStudents")]
        public async Task<IActionResult> GetStudents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GetStudents function triggered.");

            try
            {
                // Get requesting user ID from query
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new StudentListResponse
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
                    return new UnauthorizedObjectResult(new StudentListResponse
                    {
                        Success = false,
                        Message = "You do not have permission to view students."
                    });
                }

                // Optional filtering by grade
                string? gradeFilter = req.Query["grade"];
                string sqlQuery = @"
                    SELECT StudentId, StudentIdentifier, FirstName, LastName, Grade, 
                           CreatedDate, LastModified
                    FROM [dbo].[Students]";

                if (!string.IsNullOrEmpty(gradeFilter) && int.TryParse(gradeFilter, out int grade))
                {
                    sqlQuery += " WHERE Grade = @Grade";
                }

                sqlQuery += " ORDER BY LastName, FirstName";

                // Get all students
                var students = new List<StudentDto>();
                await using var cmd = new SqlCommand(sqlQuery, connection);
                
                if (!string.IsNullOrEmpty(gradeFilter) && int.TryParse(gradeFilter, out int gradeValue))
                {
                    cmd.Parameters.AddWithValue("@Grade", gradeValue);
                }

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    students.Add(MapToDto(reader));
                }

                return new OkObjectResult(new StudentListResponse
                {
                    Success = true,
                    Message = "Students retrieved successfully.",
                    Students = students,
                    TotalCount = students.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving students.");
                return new ObjectResult(new StudentListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving students."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        /// <summary>
        /// GET single student by identifier
        /// </summary>
        [Function("GetStudent")]
        public async Task<IActionResult> GetStudent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation("GetStudent function triggered for ID: {Id}", id);

            try
            {
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new StudentResponse
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
                    return new UnauthorizedObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Unauthorized."
                    });
                }

                await using var cmd = new SqlCommand(@"
                    SELECT StudentId, StudentIdentifier, FirstName, LastName, Grade, 
                           CreatedDate, LastModified
                    FROM [dbo].[Students]
                    WHERE StudentIdentifier = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new OkObjectResult(new StudentResponse
                    {
                        Success = true,
                        Message = "Student found.",
                        Student = MapToDto(reader)
                    });
                }

                return new NotFoundObjectResult(new StudentResponse
                {
                    Success = false,
                    Message = "Student not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Create/Update/Delete Student (Admin only)

        /// <summary>
        /// POST create new student - Admin only
        /// </summary>
        [Function("CreateStudent")]
        public async Task<IActionResult> CreateStudent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("CreateStudent function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<CreateStudentRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.StudentIdentifier) ||
                    string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                {
                    return new BadRequestObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Invalid request. Student identifier, first name, and last name are required."
                    });
                }

                if (request.Grade < 1 || request.Grade > 12)
                {
                    return new BadRequestObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Grade must be between 1 and 12."
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
                    return new ForbidResult("Only administrators can create students.");
                }

                // Check if student already exists
                await using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM [dbo].[Students] WHERE StudentIdentifier = @Id",
                    connection);
                checkCmd.Parameters.AddWithValue("@Id", request.StudentIdentifier);

                if ((int)(await checkCmd.ExecuteScalarAsync() ?? 0) > 0)
                {
                    return new ConflictObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "A student with this identifier already exists."
                    });
                }

                // Insert student
                await using var insertCmd = new SqlCommand(@"
                    INSERT INTO [dbo].[Students] 
                    (StudentIdentifier, FirstName, LastName, Grade, ModifiedBy, LastModified)
                    VALUES (@Id, @FirstName, @LastName, @Grade, @UserId, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

                insertCmd.Parameters.AddWithValue("@Id", request.StudentIdentifier);
                insertCmd.Parameters.AddWithValue("@FirstName", request.FirstName);
                insertCmd.Parameters.AddWithValue("@LastName", request.LastName);
                insertCmd.Parameters.AddWithValue("@Grade", request.Grade);
                insertCmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);

                int newId = (int)(await insertCmd.ExecuteScalarAsync() ?? 0);

                _logger.LogInformation("Student created with ID: {Id}", newId);

                return new OkObjectResult(new StudentResponse
                {
                    Success = true,
                    Message = "Student created successfully.",
                    Student = new StudentDto
                    {
                        StudentId = newId,
                        StudentIdentifier = request.StudentIdentifier,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        FullName = $"{request.FirstName} {request.LastName}",
                        Grade = request.Grade
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// PUT update student details - Admin only
        /// </summary>
        [Function("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequest req)
        {
            _logger.LogInformation("UpdateStudent function triggered.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonSerializer.Deserialize<UpdateStudentRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request == null || string.IsNullOrWhiteSpace(request.StudentIdentifier))
                {
                    return new BadRequestObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Invalid request."
                    });
                }

                if (request.Grade.HasValue && (request.Grade.Value < 1 || request.Grade.Value > 12))
                {
                    return new BadRequestObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Grade must be between 1 and 12."
                    });
                }

                string? connectionString = _configuration.GetConnectionString("SqlConnection");
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var accountLevel = await _authService.GetUserAccountLevel(connection, request.RequestingUserId);
                if (!_authService.CanFullyModifyUniform(accountLevel))
                {
                    return new ForbidResult("Only administrators can update student details.");
                }

                // Build dynamic update query
                var updates = new List<string>();
                var cmd = new SqlCommand { Connection = connection };

                if (!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    updates.Add("FirstName = @FirstName");
                    cmd.Parameters.AddWithValue("@FirstName", request.FirstName);
                }

                if (!string.IsNullOrWhiteSpace(request.LastName))
                {
                    updates.Add("LastName = @LastName");
                    cmd.Parameters.AddWithValue("@LastName", request.LastName);
                }

                if (request.Grade.HasValue)
                {
                    updates.Add("Grade = @Grade");
                    cmd.Parameters.AddWithValue("@Grade", request.Grade.Value);
                }

                if (updates.Count == 0)
                {
                    return new BadRequestObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "No updates specified."
                    });
                }

                updates.Add("LastModified = GETDATE()");
                updates.Add("ModifiedBy = @UserId");

                cmd.CommandText = $@"
                    UPDATE [dbo].[Students]
                    SET {string.Join(", ", updates)}
                    WHERE StudentIdentifier = @Id";

                cmd.Parameters.AddWithValue("@Id", request.StudentIdentifier);
                cmd.Parameters.AddWithValue("@UserId", request.RequestingUserId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Student not found."
                    });
                }

                return new OkObjectResult(new StudentResponse
                {
                    Success = true,
                    Message = "Student updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// DELETE student - Admin only
        /// </summary>
        [Function("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation("DeleteStudent function triggered for: {Id}", id);

            try
            {
                if (!int.TryParse(req.Query["userId"], out int userId))
                {
                    return new BadRequestObjectResult(new StudentResponse
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
                    return new ForbidResult("Only administrators can delete students.");
                }

                await using var cmd = new SqlCommand(
                    "DELETE FROM [dbo].[Students] WHERE StudentIdentifier = @Id",
                    connection);
                cmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return new NotFoundObjectResult(new StudentResponse
                    {
                        Success = false,
                        Message = "Student not found."
                    });
                }

                return new OkObjectResult(new StudentResponse
                {
                    Success = true,
                    Message = "Student deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region Helper Methods

        private StudentDto MapToDto(SqlDataReader reader)
        {
            var firstName = reader.GetString(reader.GetOrdinal("FirstName"));
            var lastName = reader.GetString(reader.GetOrdinal("LastName"));

            return new StudentDto
            {
                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                StudentIdentifier = reader.GetString(reader.GetOrdinal("StudentIdentifier")),
                FirstName = firstName,
                LastName = lastName,
                FullName = $"{firstName} {lastName}",
                Grade = reader.GetInt32(reader.GetOrdinal("Grade")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                LastModified = reader.IsDBNull(reader.GetOrdinal("LastModified"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("LastModified"))
            };
        }

        #endregion
    }
}
