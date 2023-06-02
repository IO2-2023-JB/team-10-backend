using Contracts;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using MongoDB.Driver;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIReactionRepository : MockIRepositoryBase<IReactionRepository, Reaction>
	{
		public static Mock<IReactionRepository> GetMock()
		{
			var collection = new List<Reaction>()
			{
				new Reaction()
				{
					Id = "64623f1db83bfeff70a313ad",
					VideoId = "64623f1db83bfeff70a313ac",
					UserId = "6465615b2643675169770867",
					ReactionType = ReactionType.None
				},
				new Reaction()
				{
					Id = "6465615b264367516977086A",
					VideoId = "6465615b264367516977086B",
					UserId = "6465615b264367516977086C",
					ReactionType = ReactionType.Positive
				},
				new Reaction()
				{
					Id = "6465615b264367516977086D",
					VideoId = "6465615b264367516977086E",
					UserId = "6465177ea074a4809cea03e8",
					ReactionType = ReactionType.Negative
				},
			};

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.GetCurrentUserReaction(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(
					(string videoId, string userId) =>
						collection
							.Where(o => o.UserId == userId && o.VideoId == videoId)
							.Select(x => (x.ReactionType, x.Id))
							.Single()
				);

			mock.Setup(m => m.GetReactionsCount(It.IsAny<string>()))
				.ReturnsAsync(
					(string videoId) =>
					{
						var ret = new ReactionResponseDto();
						ret.PositiveCount = collection
							.Where(o => o.VideoId == videoId && o.ReactionType == ReactionType.Positive)
							.Count();
						ret.NegativeCount = collection
							.Where(o => o.VideoId == videoId && o.ReactionType == ReactionType.Negative)
							.Count();
						return ret;
					}
				);

			return mock;
		}
	}
}
