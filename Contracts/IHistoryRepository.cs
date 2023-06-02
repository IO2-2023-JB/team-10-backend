using Entities.Models;

namespace Contracts
{
	public interface IHistoryRepository : IRepositoryBase<UserHistory>
	{
		Task AddToHistory(string userId, string videoId);
	}
}
