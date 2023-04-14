﻿using Entities.Models;

namespace Contracts
{
	public interface ISubscriptionsRepository : IRepositoryBase<Subscription>
	{
		Task<Subscription?> GetSubscription(string creatorId, string subscriberId);
		Task<IEnumerable<Subscription>> GetUserSubscriptions(string userId);
	}
}
