namespace CompsciAzureFunctionAPI2026.Models
{
    /// <summary>
    /// Request to create a new student
    /// </summary>
    public class CreateStudentRequest
    {
        public string StudentIdentifier { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Grade { get; set; }
        public int RequestingUserId { get; set; }
    }

    /// <summary>
    /// Request to update student details (Admin only)
    /// </summary>
    public class UpdateStudentRequest
    {
        public string StudentIdentifier { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Grade { get; set; }
        public int RequestingUserId { get; set; }
    }

    /// <summary>
    /// Response for student operations
    /// </summary>
    public class StudentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StudentDto? Student { get; set; }
    }

    /// <summary>
    /// Response for listing students
    /// </summary>
    public class StudentListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for Student
    /// </summary>
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string StudentIdentifier { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Grade { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
