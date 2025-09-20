using System.Security.Cryptography;
using System.Text;

namespace HRMS.Application.Common.Utitlities
{
    public class PasswordHasher
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[]? passwordHash, byte[]? passwordSalt)
        {
            if(passwordHash == null || passwordSalt == null)
                return false;

            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
