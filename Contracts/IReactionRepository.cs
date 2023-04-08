using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;

namespace Contracts
{
    public interface IReactionRepository : IRepositoryBase<Reaction>
    {
        Task<ReactionResponseDto> GetReactionsCount(string id);
    }
}
