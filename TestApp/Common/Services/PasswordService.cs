
    using BCrypt.Net;

    namespace TestApp.Services
    {
        public class PasswordService
        {
            public static string HashPassword(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }

            public static bool VerifyPassword(string password, string storedHash)
            {
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
        }
    }


