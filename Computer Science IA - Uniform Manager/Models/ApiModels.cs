namespace Computer_Science_IA___Uniform_Manager.Models
{
    /// <summary>
    /// Response model for login API
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// User information from API
    /// </summary>
    public class UserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AccountLevel { get; set; }
    }

    /// <summary>
    /// Response model for create account API
    /// </summary>
    public class CreateAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }

    /// <summary>
    /// Organization data transfer object
    /// </summary>
    public class OrganizationDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? UserAccountLevel { get; set; }
    }

    // Uniform Models
    public class CreateUniformRequest
    {
        public int OrganizationId { get; set; }
        public string UniformIdentifier { get; set; } = string.Empty;
        public int UniformType { get; set; }
        public int Size { get; set; }
        public int RequestingUserId { get; set; }
    }

    public class UpdateUniformRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public int? UniformType { get; set; }
        public int? Size { get; set; }
        public int RequestingUserId { get; set; }
    }

    public class CheckOutUniformRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public string? StudentId { get; set; }
        public bool CheckOut { get; set; }
        public int RequestingUserId { get; set; }
    }

    public class UpdateConditionsRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public int[] Conditions { get; set; } = Array.Empty<int>();
        public int RequestingUserId { get; set; }
    }

    public class UniformResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UniformDto? Uniform { get; set; }
    }

    public class UniformListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<UniformDto> Uniforms { get; set; } = new List<UniformDto>();
        public int TotalCount { get; set; }
    }

    public class UniformDto
    {
        public int UniformId { get; set; }
        public string UniformIdentifier { get; set; } = string.Empty;
        public int UniformType { get; set; }
        public string UniformTypeName { get; set; } = string.Empty;
        public int Size { get; set; }
        public bool IsCheckedOut { get; set; }
        public string? AssignedStudentId { get; set; }
        public int[] Conditions { get; set; } = Array.Empty<int>();
        public string[] ConditionNames { get; set; } = Array.Empty<string>();
        public bool IsInGoodCondition { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }
    }

    // Student Models
    public class CreateStudentRequest
    {
        public int OrganizationId { get; set; }
        public string StudentIdentifier { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Grade { get; set; }
        public int RequestingUserId { get; set; }
    }

    public class UpdateStudentRequest
    {
        public string StudentIdentifier { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Grade { get; set; }
        public int RequestingUserId { get; set; }
    }

    public class StudentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StudentDto? Student { get; set; }
    }

    public class StudentListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
        public int TotalCount { get; set; }
    }

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

    // Organization User Management Models
    public class OrganizationUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AccountLevel { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class OrganizationUsersResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrganizationUserDto>? Users { get; set; }
        public int TotalCount { get; set; }
    }

    public class UpdateUserRoleRequest
    {
        public int OrganizationId { get; set; }
        public int RequestingUserId { get; set; }
        public int TargetUserId { get; set; }
        public int NewAccountLevel { get; set; }
    }

    public class UpdateUserRoleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RemoveUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
