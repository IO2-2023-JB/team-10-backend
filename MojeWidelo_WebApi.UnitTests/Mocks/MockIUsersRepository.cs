using Contracts;
using Entities.Data.User;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIUsersRepository
	{
		public static Mock<IUsersRepository> GetMock()
		{
			var mock = new Mock<IUsersRepository>();

			var collection = new List<User>()
			{
				new User()
				{
					Id = "6429a1ee0d48bf254e17eaf7",
					Name = "UnitTestName",
					Surname = "UnitTestSurname",
					Nickname = "Unit Test User",
					AccountBalance = 123042,
					Email = "unit@test.com",
					Password = "password123",
					SubscriptionsCount = 42,
					UserType = Entities.Enums.UserType.Creator
				},
				new User()
				{
					Id = "1234a1ee0d48bf254e17eaf7",
					Name = "imie",
					Surname = "nazwisko",
					Nickname = "nick",
					AccountBalance = 10,
					Email = "user2@test.com",
					Password = "password123",
					SubscriptionsCount = 0,
					UserType = Entities.Enums.UserType.Simple
				}
			};

			mock.Setup(m => m.GetAll()).ReturnsAsync(() => collection);

			mock.Setup(m => m.GetById(It.IsAny<string>()))
				.ReturnsAsync((string id) => collection.FirstOrDefault(o => o.Id == id));

			mock.Setup(m => m.Create(It.IsAny<User>())).ReturnsAsync((User user) => user);

			mock.Setup(m => m.Delete(It.IsAny<string>()))
				.Callback(() =>
				{
					return;
				});

			mock.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<User>()))
				.ReturnsAsync((string id, User user) => user);

			mock.Setup(m => m.CheckPermissionToGetAccountBalance(It.IsAny<string>(), It.IsAny<UserDto>()))
				.Returns((string requesterId, UserDto user) => user);

			return mock;
		}
	}
}
