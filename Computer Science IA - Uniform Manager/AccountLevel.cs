using System;

namespace Computer_Science_IA___Uniform_Manager
{
    /// <summary>
    /// Represents user account privilege levels
    /// </summary>
    public enum AccountLevel
    {
        /// <summary>
        /// Administrator - Full access to all features and settings
        /// </summary>
        Administrator = 0,
        
        /// <summary>
        /// User - Standard user with limited administrative rights
        /// </summary>
        User = 1,
        
        /// <summary>
        /// Viewer - Read-only access to the system
        /// </summary>
        Viewer = 2
    }

    /// <summary>
    /// Helper class for working with AccountLevel enum
    /// </summary>
    public static class AccountLevelHelper
    {
        /// <summary>
        /// Converts an integer account level to the enum
        /// </summary>
        public static AccountLevel FromInt(int level)
        {
            if (!Enum.IsDefined(typeof(AccountLevel), level))
            {
                throw new ArgumentException($"Invalid account level: {level}. Valid values are 0 (Administrator), 1 (User), or 2 (Viewer).");
            }
            return (AccountLevel)level;
        }

        /// <summary>
        /// Gets a user-friendly description of the account level
        /// </summary>
        public static string GetDescription(AccountLevel level)
        {
            return level switch
            {
                AccountLevel.Administrator => "Administrator - Full access to all features",
                AccountLevel.User => "User - Standard access with limited admin rights",
                AccountLevel.Viewer => "Viewer - Read-only access",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets the display name for the account level
        /// </summary>
        public static string GetDisplayName(AccountLevel level)
        {
            return level switch
            {
                AccountLevel.Administrator => "Administrator",
                AccountLevel.User => "User",
                AccountLevel.Viewer => "Viewer",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Checks if a user has permission to perform an action based on required level
        /// </summary>
        public static bool HasPermission(AccountLevel userLevel, AccountLevel requiredLevel)
        {
            // Lower numeric values have higher permissions
            return (int)userLevel <= (int)requiredLevel;
        }
    }
}
