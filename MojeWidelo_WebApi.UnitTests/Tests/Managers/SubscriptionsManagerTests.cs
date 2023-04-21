using MojeWidelo_WebApi.UnitTests.Mocks;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.Tests.Managers
{
	public class SubscriptionsManagerTests
	{
		[Fact]
		public async Task GetSubscribersIdsForCreatorTest()
		{
			var subscriptionsRepo = MockISubscriptionsRepository.GetMock().Object;
			var manager = new SubscriptionsManager();

			var subscriptions = await subscriptionsRepo.GetCreatorSubscriptions("64390ed1d3768498801aa14f");
			var result = manager.GetSubscribersIds(subscriptions);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.IsAssignableFrom<IEnumerable<string>>(result);
		}

		[Fact]
		public async Task GetSubscribersIdsForSimpleUserTest()
		{
			var subscriptionsRepo = MockISubscriptionsRepository.GetMock().Object;
			var manager = new SubscriptionsManager();

			var subscriptions = await subscriptionsRepo.GetCreatorSubscriptions("64390ed1d3768498801aa05f");
			var result = manager.GetSubscribersIds(subscriptions);

			Assert.NotNull(result);
			Assert.Empty(result);
			Assert.IsAssignableFrom<IEnumerable<string>>(result);
		}

		[Fact]
		public async Task GetSubscribedUsersIds_NotEmpty()
		{
			var subscriptionsRepo = MockISubscriptionsRepository.GetMock().Object;
			var manager = new SubscriptionsManager();

			var subscriptions = await subscriptionsRepo.GetUserSubscriptions("64390ed1d3768498801aa04f");
			var result = manager.GetSubscribedUsersIds(subscriptions);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.IsAssignableFrom<IEnumerable<string>>(result);
		}

		[Fact]
		public async Task GetSubscribersIds_Empty()
		{
			var subscriptionsRepo = MockISubscriptionsRepository.GetMock().Object;
			var manager = new SubscriptionsManager();

			var subscriptions = await subscriptionsRepo.GetUserSubscriptions("64390ed1d3768498801aa05f");
			var result = manager.GetSubscribedUsersIds(subscriptions);

			Assert.NotNull(result);
			Assert.Empty(result);
			Assert.IsAssignableFrom<IEnumerable<string>>(result);
		}
	}
}
