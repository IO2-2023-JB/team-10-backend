namespace Entities.Data.Subscription
{
	public class SubscriptionListDto
	{
		public SubscriptionListDto(IEnumerable<SubscriptionDto> subscriptions)
		{
			Subscriptions = subscriptions;
		}

		public IEnumerable<SubscriptionDto> Subscriptions { get; set; }
	}
}
