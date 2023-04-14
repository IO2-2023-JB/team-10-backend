using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockISubscriptionsRepository : MockIRepositoryBase<ISubscriptionsRepository, Subscription>
	{
		public static Mock<ISubscriptionsRepository> GetMock()
		{
			var collection = new List<Subscription>()
			{
				new Subscription()
				{
					Id = "34293474c00e3e73ccd89803",
					CreatorId = "64390ed1d3768498801aa14f",
					SubscriberId = "64390ed1d3768498801aa03f"
				},
				new Subscription()
				{
					Id = "55590ed1d3768498801aa04f",
					CreatorId = "64390ed1d3768498801aa14f",
					SubscriberId = "64390ed1d3768498801aa04f"
				}
			};

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.GetSubscription(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(
					(string creatorId, string subscriberId) =>
						collection
							.Where(s => s.CreatorId == creatorId && s.SubscriberId == subscriberId)
							.FirstOrDefault()
				);

			mock.Setup(m => m.GetUserSubscriptions(It.IsAny<string>()))
				.ReturnsAsync((string userId) => collection.Where(s => s.CreatorId == userId).ToList());

			return mock;
		}
	}
}
