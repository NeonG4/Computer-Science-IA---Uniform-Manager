namespace CompsciAzureFunctionAPI2026.Models
{
    public class Organization
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class UserOrganization
    {
        public int UserOrganizationId { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int AccountLevel { get; set; } // 0=Admin, 1=User, 2=Viewer
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class OrganizationDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? UserAccountLevel { get; set; } // User's role in this org
    }

    public class CreateOrganizationRequest
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserId { get; set; }
    }

    public class CreateOrganizationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public OrganizationDto? Organization { get; set; }
    }

    public class GetOrganizationsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrganizationDto>? Organizations { get; set; }
        public int TotalCount { get; set; }
    }
}
