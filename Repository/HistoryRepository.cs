using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Driver;

namespace Repository
{
	public class HistoryRepository : RepositoryBase<UserHistory>, IHistoryRepository
	{
		public HistoryRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.HistoryCollectionName) { }

		public async Task AddToHistory(string userId, string videoId)
		{
			var update = Builders<UserHistory>.Update.Push<HistoryItem>(x => x.WatchedVideos, new HistoryItem(videoId));
			await Collection.UpdateOneAsync(x => x.Id == userId, update);
		}
	}
}
