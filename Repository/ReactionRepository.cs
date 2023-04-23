using Contracts;
using Entities.Data.Video;
using Entities.Utils;
using Entities.Enums;
using Entities.Models;
using MongoDB.Driver;

namespace Repository
{
	public class ReactionRepository : RepositoryBase<Reaction>, IReactionRepository
	{
		public ReactionRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.ReactionCollectionName) { }

		public async Task<ReactionResponseDto> GetReactionsCount(string id)
		{
			var reactions = await GetAll();
			var ret = new ReactionResponseDto();
			ret.PositiveCount = reactions.Count((x) => x.VideoId == id && x.ReactionType == ReactionType.Positive);
			ret.NegativeCount = reactions.Count((x) => x.VideoId == id && x.ReactionType == ReactionType.Negative);
			return ret;
		}

		public async Task<(ReactionType reactionType, string id)> GetCurrentUserReaction(string videoId, string userId)
		{
			var item = (
				await _collection.Find((x) => x.VideoId == videoId && x.UserId == userId).ToListAsync()
			).FirstOrDefault();

			if (item == null)
				return (ReactionType.None, "");
			return (item.ReactionType, item.Id);
		}
	}
}
