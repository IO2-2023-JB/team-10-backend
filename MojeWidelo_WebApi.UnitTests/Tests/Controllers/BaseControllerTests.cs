using AutoMapper;
using Contracts;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using MojeWidelo_WebApi.Mapper;
using MojeWidelo_WebApi.UnitTests.Mocks;
using Moq;
using System.Security.Claims;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public abstract class BaseControllerTests<T>
		where T : BaseController
	{
		protected abstract T GetController();

		protected Mock<IRepositoryWrapper> GetRepositoryWrapperMock()
		{
			return MockIRepositoryWrapper.GetMock();
		}

		protected IMapper GetMapper()
		{
			var configuration = new MapperConfiguration(cfs =>
			{
				cfs.AddProfile(new UserProfile());
				cfs.AddProfile(new VideoProfile());
				cfs.AddProfile(new SubscriptionProfile());
				cfs.AddProfile(new CommentProfile());
				cfs.AddProfile(new PlaylistProfile());
			});
			return new AutoMapper.Mapper(configuration);
		}

		protected ControllerContext GetControllerContext(
			string? id = null,
			UserType? userType = null,
			string? nickname = null
		)
		{
			var httpContext = new DefaultHttpContext()
			{
				User = new ClaimsPrincipal(
					new ClaimsIdentity(
						new Claim[]
						{
							new Claim(ClaimTypes.NameIdentifier, id ?? MockUser.Id),
							new Claim(ClaimTypes.Role, (userType ?? MockUser.UserType).ToString()),
							new Claim(ClaimTypes.Name, nickname ?? MockUser.Nickname),
						}
					)
				)
			};

			return new ControllerContext() { HttpContext = httpContext };
		}
	}
}
