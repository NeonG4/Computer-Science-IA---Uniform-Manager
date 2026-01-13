using System.Security.Cryptography;
using System.Text;

namespace CompsciAzureFunctionAPI2026.Services
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
            return Encoding.ASCII.GetString(hashedBytes);
        }
    }
}
