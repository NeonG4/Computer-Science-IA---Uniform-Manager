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
}
