using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CompsciAzureFunctionAPI2026.Services
{
    /// <summary>
    /// Service for checking user permissions
    /// </summary>
    public class AuthorizationService
    {
        private readonly ILogger _logger;

        public AuthorizationService(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Account levels matching the database
        /// </summary>
        public enum AccountLevel
        {
            Administrator = 0,
            User = 1,
            Viewer = 2
        }

        /// <summary>
        /// Gets the account level for a user from the database
        /// </summary>
        public async Task<AccountLevel?> GetUserAccountLevel(SqlConnection connection, int userId)
        {
            try
            {
                await using var cmd = new SqlCommand(
                    "SELECT AccountLevel FROM [dbo].[UserInfo] WHERE UserId = @UserId",
                    connection);
                cmd.Parameters.AddWithValue("@UserId", userId);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return (AccountLevel)(int)result;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user account level for UserId: {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Checks if user is an administrator
        /// </summary>
        public bool IsAdministrator(AccountLevel? level)
        {
            return level == AccountLevel.Administrator;
        }

        /// <summary>
        /// Checks if user is at least a User (User or Admin)
        /// </summary>
        public bool IsUserOrHigher(AccountLevel? level)
        {
            return level == AccountLevel.User || level == AccountLevel.Administrator;
        }

        /// <summary>
        /// Checks if user has read permission (all levels can read)
        /// </summary>
        public bool CanRead(AccountLevel? level)
        {
            return level.HasValue; // All authenticated users can read
        }

        /// <summary>
        /// Checks if user can modify check-out status and conditions
        /// </summary>
        public bool CanModifyCheckOutAndConditions(AccountLevel? level)
        {
            return IsUserOrHigher(level);
        }

        /// <summary>
        /// Checks if user can fully modify uniform (create, delete, change type/size)
        /// </summary>
        public bool CanFullyModifyUniform(AccountLevel? level)
        {
            return IsAdministrator(level);
        }
    }
}
