using System;
using System.Security.Cryptography;
using System.Text;

namespace BarberShopManagement.Presentation.Utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Tạo hash từ password
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                // Chuyển đổi sang chuỗi HEX viết thường (giống MySQL SHA2)
                StringBuilder hexBuilder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    hexBuilder.Append(b.ToString("x2")); // "x2" = hex 2 ký tự viết thường
                }
                return hexBuilder.ToString();
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Hash password nhập vào và so sánh với hash trong DB
            string inputHash = HashPassword(inputPassword);
            return string.Equals(inputHash, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
