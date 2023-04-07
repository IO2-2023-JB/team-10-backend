using Entities.Enums;
using Entities.Models;

namespace Contracts
{
    public interface IReactionRepository : IRepositoryBase<Reaction>
    {
        Task<(int positiveCount, int negativeCount)> GetReactionsCount(string id);
    }
}
