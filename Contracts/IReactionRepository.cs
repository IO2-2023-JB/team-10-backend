using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;

namespace Contracts
{
	public interface IReactionRepository : IRepositoryBase<Reaction>
	{
		Task<ReactionResponseDto> GetReactionsCount(string id);

		Task<(ReactionType reactionType, string id)> GetCurrentUserReaction(string videoId, string userId);
	}
}
