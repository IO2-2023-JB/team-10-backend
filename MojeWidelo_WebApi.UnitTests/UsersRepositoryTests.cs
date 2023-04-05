using Entities.Data.User;
using Entities.Models;
using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests
{
	public class UsersRepositoryTests
	{
		[Theory]
		[InlineData("unit@test.com")]
		[InlineData("user2@test.com")]
		public async void FindUserByEmailTest(string email)
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();
			var result = await repositoryWrapperMock.Object.UsersRepository.FindUserByEmail(email);

			Assert.NotNull(result);
			Assert.Equal(email, result.Email);
			Assert.IsAssignableFrom<User>(result);
		}

		[Theory]
		[InlineData("jdsiajfiweo@test.com")]
		[InlineData("halohalo@halo.com")]
		public async void FindUserByNonExistingEmailTest(string email)
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();
			var result = await repositoryWrapperMock.Object.UsersRepository.FindUserByEmail(email);

			Assert.Null(result);
		}

		[Theory]
		[InlineData("1234a1ee0d48bf254e17eaf7")]
		public void CheckPermissionToGetAccountBalanceTestHasPermissions(string requesterId)
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();

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

			var result = repositoryWrapperMock.Object.UsersRepository.CheckPermissionToGetAccountBalance(
				requesterId,
				user
			);

			Assert.NotNull(result);
			Assert.NotNull(result.AccountBalance);
		}

		[Theory]
		[InlineData("414235463fdsdsafdasfd323")]
		[InlineData("6789a1ee0d48bf254e17eaf7")]
		public void CheckPermissionToGetAccountBalanceTestNoPermissions(string requesterId)
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();

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

			var result = repositoryWrapperMock.Object.UsersRepository.CheckPermissionToGetAccountBalance(
				requesterId,
				user
			);

			Assert.NotNull(result);
			Assert.Null(result.AccountBalance);
		}
	}
}
