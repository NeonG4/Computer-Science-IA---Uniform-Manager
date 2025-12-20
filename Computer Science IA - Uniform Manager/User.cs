using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Science_IA___Uniform_Manager
{
    public class User
    {
        private string fName;

        private string lName;
        private string email;
        private AccountPrivileges accountPrivileges;
        private string hashedPassword;
        public User(string fName, string lName, string email, string hashedPassword)
        {
            this.fName = fName;
            this.lName = lName;
            this.email = email;
            this.hashedPassword = hashedPassword;
            this.accountPrivileges = AccountPrivileges.Viewer; // Default privilege
        }
        public bool CheckHashedPassword(string hashedPassword)
        {
            return this.hashedPassword == hashedPassword;
        }
        public void PromoteUser(User user, string hashedPassword, AccountPrivileges level, out bool promoted)
        {
            promoted = false;
            if (this.accountPrivileges != AccountPrivileges.Administrator)
            {
                return;
            }
            if (user.CheckHashedPassword(hashedPassword))
            {
                this.accountPrivileges = level;
                promoted = true;
            }
        }
        public string GetFirstName()
        {
            return fName;
        }
        public string GetLastName()
        {
            return lName;
        }
        public string GetEmail()
        {
            return email;
        }
        public AccountPrivileges GetAccountPrivileges()
        {
            return accountPrivileges;
        }
        public string GetFullName()
        {
            return $"{fName} {lName}";

        }
    }
    public enum AccountPrivileges
    {
        Administrator,
        User,
        Viewer
    }
}
