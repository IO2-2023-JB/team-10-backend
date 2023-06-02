using Entities.Models;

namespace Contracts
{
	public interface IHistoryRepository : IRepositoryBase<UserHistory>
	{
		Task AddToHistory(string userId, string videoId);
		Task<DateTime?> GetDateTimeOfLastWatchedVideoById(string userId, string videoId);
	}
}
