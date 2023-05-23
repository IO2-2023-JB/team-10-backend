using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Driver;

namespace Repository
{
	public class HistoryRepository : RepositoryBase<History>, IHistoryRepository
	{
		public HistoryRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.HistoryCollectionName) { }

		public async Task AddToHistory(string userId, string videoId)
		{
			var historyItem = new HistoryItem(videoId);
			var update = Builders<History>.Update.Push<HistoryItem>(x => x.WatchedVideos, historyItem);
			await Collection.UpdateOneAsync(x => x.Id == userId, update);
		}
	}
}
