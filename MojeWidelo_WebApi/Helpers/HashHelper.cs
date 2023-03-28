using BC = BCrypt.Net.BCrypt;

namespace MojeWidelo_WebApi.Helpers
{
    public static class HashHelper
    {
        public static string HashPassword(string password)
        {
            return BC.EnhancedHashPassword(password, BCrypt.Net.HashType.SHA256);
        }

        public static bool ValidatePassword(string providedPassword, string hashedPassword)
        {
            return BC.EnhancedVerify(providedPassword, hashedPassword, BCrypt.Net.HashType.SHA256);
        }
    }
}
