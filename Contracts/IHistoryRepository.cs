using Entities.Models;

namespace Contracts
{
	public interface IHistoryRepository : IRepositoryBase<History>
	{
		Task AddToHistory(string userId, string videoId);
	}
}
