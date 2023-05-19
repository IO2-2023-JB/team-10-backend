using AutoMapper;
using MojeWidelo_WebApi.Mapper;

namespace MojeWidelo_WebApi.UnitTests.Tests.Managers
{
	public class BaseManagerTests
	{
		protected IMapper GetMapper()
		{
			var configuration = new MapperConfiguration(cfs =>
			{
				cfs.AddProfile(new UserProfile());
				cfs.AddProfile(new VideoProfile());
				cfs.AddProfile(new SubscriptionProfile());
				cfs.AddProfile(new CommentProfile());
			});
			return new AutoMapper.Mapper(configuration);
		}
	}
}
