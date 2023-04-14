using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests.Tests.Repositories
{
	public class SubscriptionsRepositoryTests
	{
		[Theory]
		[InlineData("64390ed1d3768498801aa14f", "64390ed1d3768498801aa03f")]
		[InlineData("64390ed1d3768498801aa14f", "64390ed1d3768498801aa04f")]
		public async Task GetSubscription_WhenSubscribed(string creatorId, string subscriberId)
		{
			var repository = MockISubscriptionsRepository.GetMock().Object;
			var result = await repository.GetSubscription(creatorId, subscriberId);

			Assert.NotNull(result);
			Assert.Equal(result!.CreatorId, creatorId);
			Assert.Equal(result!.SubscriberId, subscriberId);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f", "64390ed1d3768498801aa05f")]
		public async Task GetSubscriptionTest_WhenNotSubscribed(string creatorId, string subscriberId)
		{
			var repository = MockISubscriptionsRepository.GetMock().Object;
			var result = await repository.GetSubscription(creatorId, subscriberId);

			Assert.Null(result);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f")]
		public async Task GetUserSubscriptionsTest_ForCreatorWithSubscriptions(string userId)
		{
			var repository = MockISubscriptionsRepository.GetMock().Object;
			var result = await repository.GetUserSubscriptions(userId);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			foreach (var sub in result)
			{
				Assert.Equal(userId, sub.CreatorId);
			}
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa15f")]
		public async Task GetUserSubscriptionsTest_ForCreatorWithoutSubscriptions(string userId)
		{
			var repository = MockISubscriptionsRepository.GetMock().Object;
			var result = await repository.GetUserSubscriptions(userId);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa03f")]
		[InlineData("64390ed1d3768498801aa04f")]
		public async Task GetUserSubscriptionsTest_ForSimpleUser(string userId)
		{
			var repository = MockISubscriptionsRepository.GetMock().Object;
			var result = await repository.GetUserSubscriptions(userId);

			Assert.NotNull(result);
			Assert.Empty(result);
		}
	}
}
