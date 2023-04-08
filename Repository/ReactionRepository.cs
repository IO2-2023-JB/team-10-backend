using Contracts;
using Entities.Data.Video;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;

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
            ret.positiveCount = reactions.Count((x) => x.ReactionType == ReactionType.Positive);
            ret.negativeCount = reactions.Count((x) => x.ReactionType == ReactionType.Negative);
            return ret;
        }
    }
}
