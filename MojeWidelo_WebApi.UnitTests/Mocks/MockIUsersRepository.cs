using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIUsersRepository : MockIRepositoryBase<IUsersRepository, User>
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
					AccountBalance = 100.0M,
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
				},
				new User()
				{
					Id = "64390ed1d3768498801aa03f",
					Name = "SubscriptionTestUser1",
					Surname = "SubscriptionTestUser1",
					Nickname = "SubTestUser1",
					AccountBalance = 0,
					Email = "sub1@unittest.com",
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
					SubscriptionsCount = 0,
					UserType = Entities.Enums.UserType.Simple
				},
				new User()
				{
					Id = "64390ed1d3768498801aa04f",
					Name = "SubscriptionTestUser2",
					Surname = "SubscriptionTestUser2",
					Nickname = "SubTestUser2",
					AccountBalance = 0,
					Email = "sub2@unittest.com",
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
					SubscriptionsCount = 0,
					UserType = Entities.Enums.UserType.Simple
				},
				new User()
				{
					Id = "64390ed1d3768498801aa05f",
					Name = "SubscriptionTestUser3",
					Surname = "SubscriptionTestUser3",
					Nickname = "SubTestUser3",
					AccountBalance = 0,
					Email = "sub3@unittest.com",
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
					SubscriptionsCount = 10,
					UserType = Entities.Enums.UserType.Simple
				},
				new User()
				{
					Id = "64390ed1d3768498801aa14f",
					Name = "creator with subs",
					Surname = "SubscriptionTestUser4",
					Nickname = "SubTestUser4",
					AccountBalance = 0,
					Email = "sub4@unittest.com",
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
					SubscriptionsCount = 0,
					UserType = Entities.Enums.UserType.Creator
				},
				new User()
				{
					Id = "64390ed1d3768498801aa15f",
					Name = "creator without subs",
					Surname = "SubscriptionTestUser5",
					Nickname = "SubTestUser5",
					AccountBalance = 0,
					Email = "sub4@unittest.com",
					Password = "$2a$11$g6G3sI7tF3ZdJ5syUj4aLuDAMn7w2A2XS7wefOwYi/1u/.bPa3GQ6",
					SubscriptionsCount = 0,
					UserType = Entities.Enums.UserType.Creator
				},
			};

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.FindUserByEmail(It.IsAny<string>()))
				.ReturnsAsync((string email) => collection.FirstOrDefault(o => o.Email == email)!);

			mock.Setup(m => m.GetUsersByIds(It.IsAny<IEnumerable<string>>()))
				.ReturnsAsync((IEnumerable<string> ids) => collection.Where(user => ids.Contains(user.Id)).ToList());

			mock.Setup(m => m.UpdateAccountBalance(It.IsAny<string>(), It.IsAny<decimal>())).Callback(() => { });

			return mock;
		}
	}
}
