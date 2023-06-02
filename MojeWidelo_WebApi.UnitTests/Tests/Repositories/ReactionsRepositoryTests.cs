using Entities.Data.Video;
using Entities.Enums;
using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests.Tests.Repositories
{
	public class ReactionsRepositoryTests
	{
		[Theory]
		[InlineData("64623f1db83bfeff70a313ac", "6465615b2643675169770867")]
		public async void GetCurrenUserReactionNone(string userId, string videoId)
		{
			var reactionCollectionMock = MockIReactionRepository.GetMock();
			var result = await reactionCollectionMock.Object.GetCurrentUserReaction(userId, videoId);
			Assert.IsAssignableFrom<ValueTuple<ReactionType, string>>(result);
			Assert.True(result.reactionType == ReactionType.None);
		}

		[Theory]
		[InlineData("6465615b264367516977086B", "6465615b264367516977086C")]
		public async void GetCurrenUserReactionPositive(string userId, string videoId)
		{
			var reactionCollectionMock = MockIReactionRepository.GetMock();
			var result = await reactionCollectionMock.Object.GetCurrentUserReaction(userId, videoId);
			Assert.IsAssignableFrom<ValueTuple<ReactionType, string>>(result);
			Assert.True(result.reactionType == ReactionType.Positive);
		}

		[Theory]
		[InlineData("6465615b264367516977086E", "6465177ea074a4809cea03e8")]
		public async void GetCurrenUserReactionNegative(string userId, string videoId)
		{
			var reactionCollectionMock = MockIReactionRepository.GetMock();
			var result = await reactionCollectionMock.Object.GetCurrentUserReaction(userId, videoId);
			Assert.IsAssignableFrom<ValueTuple<ReactionType, string>>(result);
			Assert.True(result.reactionType == ReactionType.Negative);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async void GetReactionsCountNone(string videoId)
		{
			var reactionCollectionMock = MockIReactionRepository.GetMock();
			var result = await reactionCollectionMock.Object.GetReactionsCount(videoId);
			Assert.IsAssignableFrom<ReactionResponseDto>(result);
			Assert.True(result.PositiveCount == 0 && result.NegativeCount == 0);
		}

		[Theory]
		[InlineData("6465615b264367516977086B")]
		public async void GetReactionsCountPositive(string videoId)
		{
			var reactionCollectionMock = MockIReactionRepository.GetMock();
			var result = await reactionCollectionMock.Object.GetReactionsCount(videoId);
			Assert.IsAssignableFrom<ReactionResponseDto>(result);
			Assert.True(result.PositiveCount == 1 && result.NegativeCount == 0);
		}

		[Theory]
		[InlineData("6465615b264367516977086E")]
		public async void GetReactionsCountNegative(string videoId)
		{
			var reactionCollectionMock = MockIReactionRepository.GetMock();
			var result = await reactionCollectionMock.Object.GetReactionsCount(videoId);
			Assert.IsAssignableFrom<ReactionResponseDto>(result);
			Assert.True(result.PositiveCount == 0 && result.NegativeCount == 1);
		}
	}
}
