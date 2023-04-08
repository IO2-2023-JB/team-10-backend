using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIUsersRepository
	{
		public static Mock<IUsersRepository> GetMock()
		{
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
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
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
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
					SubscriptionsCount = 0,
					UserType = Entities.Enums.UserType.Simple
				}
			};

			var mock = new Mock<IUsersRepository>();

			mock.Setup(m => m.GetAll()).ReturnsAsync(() => collection);

			mock.Setup(m => m.GetById(It.IsAny<string>()))
				.ReturnsAsync((string id) => collection.FirstOrDefault(o => o.Id == id)!);

			mock.Setup(m => m.Create(It.IsAny<User>())).ReturnsAsync((User user) => user);

			mock.Setup(m => m.Delete(It.IsAny<string>())).Callback(() => { });

			mock.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<User>()))
				.ReturnsAsync((string id, User user) => user);

			mock.Setup(m => m.FindUserByEmail(It.IsAny<string>()))
				.ReturnsAsync((string email) => collection.FirstOrDefault(o => o.Email == email)!);

			return mock;
		}
	}
}
