using Entities.Data.Subscription;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
	public class SubscriptionProfile : AutoMapper.Profile
	{
		public SubscriptionProfile()
		{
			CreateMap<User, SubscriptionDto>();
		}
	}
}
