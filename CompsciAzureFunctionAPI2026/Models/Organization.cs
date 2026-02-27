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
        public int AccountLevel { get; set; } // 0=Admin, 1=User, 2=Viewer, required for number for DB
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

    public class JoinOrganizationRequest
    {
        public string OrganizationCode { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int RequestedAccountLevel { get; set; } // 0=Admin, 1=User, 2=Viewer
        public string? RequestMessage { get; set; } // Optional message to admins
    }

    public class JoinOrganizationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RequiresApproval { get; set; } // True if request is pending approval
    }

    public class OrganizationJoinRequest
    {
        public int RequestId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int RequestedAccountLevel { get; set; }
        public int Status { get; set; } // 0=Pending, 1=Approved, 2=Rejected
        public string? RequestMessage { get; set; }
        public DateTime RequestedDate { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewNotes { get; set; }
    }

    public class GetJoinRequestsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrganizationJoinRequest>? Requests { get; set; }
        public int TotalCount { get; set; }
    }

    public class ReviewJoinRequestRequest
    {
        public int RequestId { get; set; }
        public int ReviewerId { get; set; }
        public bool Approved { get; set; }
        public string? ReviewNotes { get; set; }
    }

    public class ReviewJoinRequestResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class GetOrganizationsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrganizationDto>? Organizations { get; set; }
        public int TotalCount { get; set; }
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

    public class GetOrganizationUsersResponse
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
        public int NewAccountLevel { get; set; } // 0=Admin, 1=User, 2=Viewer
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
