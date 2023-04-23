using Entities.Data.User;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.Tests.Managers
{
	public class UsersManagerTests
	{
		[Theory]
		[InlineData("1234a1ee0d48bf254e17eaf7")]
		public void CheckPermissionToGetAccountBalanceTestHasPermissions(string requesterId)
		{
			var usersManager = new UsersManager();

			var user = new UserDto()
			{
				Id = "1234a1ee0d48bf254e17eaf7",
				Name = "imie",
				Surname = "nazwisko",
				Nickname = "nick",
				AccountBalance = 10,
				Email = "user2@test.com",
				SubscriptionsCount = 0,
				UserType = Entities.Enums.UserType.Simple
			};

			var result = usersManager.CheckPermissionToGetAccountBalance(requesterId, user);

			Assert.NotNull(result);
			Assert.NotNull(result.AccountBalance);
		}

		[Theory]
		[InlineData("414235463fdsdsafdasfd323")]
		[InlineData("6789a1ee0d48bf254e17eaf7")]
		public void CheckPermissionToGetAccountBalanceTestNoPermissions(string requesterId)
		{
			var usersManager = new UsersManager();

			var user = new UserDto()
			{
				Id = "1234a1ee0d48bf254e17eaf7",
				Name = "imie",
				Surname = "nazwisko",
				Nickname = "nick",
				AccountBalance = 10,
				Email = "user2@test.com",
				SubscriptionsCount = 0,
				UserType = Entities.Enums.UserType.Simple
			};

			var result = usersManager.CheckPermissionToGetAccountBalance(requesterId, user);

			Assert.NotNull(result);
			Assert.Null(result.AccountBalance);
		}
	}
}
