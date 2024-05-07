using System.Security.Cryptography;
using System.Text;

namespace Payroll.Core.Helpers
{
    public static class GeneratorHelper
    {
        private const string _salt = "r7hzcu5wWS25uKx";
        public static string GenPasswordSha256(this string password)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                string mixPassword = $"{password}{_salt}";
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(mixPassword));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static bool CheckPassword(this string password, string sha256Password)
        {
            string resPassword = GenPasswordSha256(password);
            return resPassword == sha256Password;
        }
        public static string GenCode(this string prefix, int num, byte step, byte space = 8)
        {
            string nextNum = (num + step).ToString().PadLeft(space, '0');
            string code = $"{prefix}{nextNum}";
            return code;
        }
        public static string GenSubCode(this string code, string separator, int num, byte step, byte space = 4)
        {
            string nextNum = (num + step).ToString().PadLeft(space, '0');
            string fullCode = $"{code}{separator}{nextNum}";
            return fullCode;
        }
    }
}
