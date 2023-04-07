using Contracts;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;
using System.Runtime.InteropServices;

namespace Repository
{
    public class ReactionRepository : RepositoryBase<Reaction>, IReactionRepository
    {
        public ReactionRepository(IDatabaseSettings databaseSettings)
            : base(databaseSettings, databaseSettings.VideoCollectionName) { }

        public async Task<(int positiveCount, int negativeCount)> GetReactionsCount(string id)
        {
            var reactions = await GetAll();
            return (reactions.Count((x) => x.ReactionType == ReactionType.Positive), reactions.Count((x) => x.ReactionType == ReactionType.Negative));
        }
    }
}
