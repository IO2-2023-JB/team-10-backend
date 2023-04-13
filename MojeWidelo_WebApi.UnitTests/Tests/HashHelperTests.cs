using MojeWidelo_WebApi.Helpers;

namespace MojeWidelo_WebApi.UnitTests.Tests
{
	public class HashHelperTests
	{
		[Theory]
		[InlineData("")]
		[InlineData("password123")]
		[InlineData("fudsuf 932nr")]
		[InlineData("4732rhfsudidnf9832hsdjkf023")]
		public void HashPasswordTest(string password)
		{
			var hashedPassword = HashHelper.HashPassword(password);

			Assert.NotNull(hashedPassword);
			Assert.NotEmpty(hashedPassword);
			Assert.NotEqual(password, hashedPassword);
		}

		[Theory]
		[InlineData("")]
		[InlineData("password123")]
		[InlineData("fudsuf 932nr")]
		[InlineData("4732rhfsudidnf9832hsdjkf023")]
		public void ValidatePasswordSuccessTest(string password)
		{
			var hashedPassword = HashHelper.HashPassword(password);

			var isPasswordValid = HashHelper.ValidatePassword(password, hashedPassword);

			Assert.True(isPasswordValid);
		}

		[Theory]
		[InlineData("djsaifjoindsjn", "jidashei")]
		[InlineData("password123", "password1233")]
		public void ValidatePasswordFailTest(string password, string password2)
		{
			var hashedPassword = HashHelper.HashPassword(password);

			var isPasswordValid = HashHelper.ValidatePassword(password2, hashedPassword);

			Assert.NotEqual(password, hashedPassword);
			Assert.False(isPasswordValid);
		}
	}
}
