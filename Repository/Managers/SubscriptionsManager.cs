using Entities.Models;

namespace Repository.Managers
{
	public class SubscriptionsManager
	{
		public IEnumerable<string> GetSubscribersIds(IEnumerable<Subscription> subscriptions)
		{
			return subscriptions.Select(s => s.SubscriberId);
		}

		public IEnumerable<string> GetSubscribedUsersIds(IEnumerable<Subscription> subscriptions)
		{
			return subscriptions.Select(s => s.CreatorId);
		}
	}
}
