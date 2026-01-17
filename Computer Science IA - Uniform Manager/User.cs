using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Science_IA___Uniform_Manager
{
    /// <summary>
    /// Represents a user account in the system
    /// </summary>
    public class User
    {
        public int UserId { get; }
        public string Username { get; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public AccountLevel AccountLevel { get; private set; }
        
        private string hashedPassword;

        public User(int userId, string username, string firstName, string lastName, string email, string hashedPassword, AccountLevel accountLevel = AccountLevel.Viewer)
        {
            UserId = userId;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            this.hashedPassword = hashedPassword;
            AccountLevel = accountLevel;
        }

        public string FullName => $"{FirstName} {LastName}";

        public bool CheckHashedPassword(string hashedPassword)
        {
            return this.hashedPassword == hashedPassword;
        }

        /// <summary>
        /// Promotes a user to a new account level (Administrator only)
        /// </summary>
        public bool PromoteUser(User user, string adminPassword, AccountLevel newLevel)
        {
            if (AccountLevel != AccountLevel.Administrator)
            {
                return false;
            }
            
            if (!CheckHashedPassword(adminPassword))
            {
                return false;
            }
            
            user.AccountLevel = newLevel;
            return true;
        }

        public void SetAccountLevel(AccountLevel level)
        {
            AccountLevel = level;
        }

        public bool HasPermission(AccountLevel requiredLevel)
        {
            return AccountLevelHelper.HasPermission(AccountLevel, requiredLevel);
        }

        public bool IsAdministrator() => AccountLevel == AccountLevel.Administrator;

        public bool IsUser() => AccountLevel == AccountLevel.User;

        public bool IsViewer() => AccountLevel == AccountLevel.Viewer;
    }
}
