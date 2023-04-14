using Entities.Models;

namespace Repository.Managers
{
	public class SubscriptionsManager
	{
		public async Task<IEnumerable<string>> GetSubscribersIds(Task<IEnumerable<Subscription>> task)
		{
			var subscriptions = await task;
			return subscriptions.Select(s => s.SubscriberId);
		}
	}
}
