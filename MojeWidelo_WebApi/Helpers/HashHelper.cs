using BCrypt.Net;
using BC = BCrypt.Net.BCrypt;

namespace MojeWidelo_WebApi.Helpers
{
	public static class HashHelper
	{
		private const HashType _hashType = HashType.SHA256;

		public static string HashPassword(string password)
		{
			return BC.EnhancedHashPassword(password, _hashType);
		}

		public static bool ValidatePassword(string providedPassword, string hashedPassword)
		{
			return BC.EnhancedVerify(providedPassword, hashedPassword, _hashType);
		}
	}
}
