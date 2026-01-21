namespace CompsciAzureFunctionAPI2026.Models
{
    /// <summary>
    /// Request to create a new uniform
    /// </summary>
    public class CreateUniformRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public int UniformType { get; set; } // UniformClothing enum value
        public int Size { get; set; }
        public int RequestingUserId { get; set; }
    }

    /// <summary>
    /// Request to update uniform details (Admin only)
    /// </summary>
    public class UpdateUniformRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public int? UniformType { get; set; }
        public int? Size { get; set; }
        public int RequestingUserId { get; set; }
    }

    /// <summary>
    /// Request to check out/in a uniform (User/Admin)
    /// </summary>
    public class CheckOutUniformRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public string? StudentId { get; set; }
        public bool CheckOut { get; set; } // true = check out, false = check in
        public int RequestingUserId { get; set; }
    }

    /// <summary>
    /// Request to update uniform conditions (User/Admin)
    /// </summary>
    public class UpdateConditionsRequest
    {
        public string UniformIdentifier { get; set; } = string.Empty;
        public int[] Conditions { get; set; } = Array.Empty<int>(); // Condition enum values
        public int RequestingUserId { get; set; }
    }

    /// <summary>
    /// Response for uniform operations
    /// </summary>
    public class UniformResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UniformDto? Uniform { get; set; }
    }

    /// <summary>
    /// Response for listing uniforms
    /// </summary>
    public class UniformListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<UniformDto> Uniforms { get; set; } = new List<UniformDto>();
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for Uniform
    /// </summary>
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

    /// <summary>
    /// Enums matching the C# classes
    /// </summary>
    public enum UniformClothing
    {
        ConcertCoat = 0,
        DrumMajorCoat = 1,
        Hat = 2,
        MarchingCoat = 3,
        MarchingShorts = 4,
        MarchingSocks = 5,
        Pants = 6
    }

    public enum Condition
    {
        Stain = 0,
        BrokenButton = 1,
        BrokenZipper = 2,
        Torn = 3,
        Missing = 4,
        Faded = 5
    }
}
