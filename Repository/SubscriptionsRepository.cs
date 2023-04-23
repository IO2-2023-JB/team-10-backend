using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Driver;

namespace Repository
{
	public class SubscriptionsRepository : RepositoryBase<Subscription>, ISubscriptionsRepository
	{
		public SubscriptionsRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.SubscriptionCollectionName) { }

		public async Task<Subscription?> GetSubscription(string creatorId, string subscriberId)
		{
			return await _collection
				.Find(s => s.CreatorId == creatorId && s.SubscriberId == subscriberId)
				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<Subscription>> GetCreatorSubscriptions(string userId)
		{
			return await _collection.Find(s => s.CreatorId == userId).ToListAsync();
		}

		public async Task<IEnumerable<Subscription>> GetUserSubscriptions(string userId)
		{
			return await _collection.Find(s => s.SubscriberId == userId).ToListAsync();
		}
	}
}
