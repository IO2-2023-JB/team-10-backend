using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Subscription
{
	public class SubscriptionListDto
	{
		public SubscriptionListDto(IEnumerable<SubscriptionDto> subscriptions)
		{
			Subscriptions = subscriptions;
		}

		[Required]
		public IEnumerable<SubscriptionDto> Subscriptions { get; set; }
	}
}
