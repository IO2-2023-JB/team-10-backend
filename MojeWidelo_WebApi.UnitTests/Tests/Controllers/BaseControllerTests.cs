using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Mapper;
using MojeWidelo_WebApi.UnitTests.Mocks;
using Moq;
using System.Security.Claims;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public abstract class BaseControllerTests<T>
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
			});
			return new AutoMapper.Mapper(configuration);
		}

		protected ControllerContext GetControllerContext()
		{
			var httpContext = new DefaultHttpContext()
			{
				User = new ClaimsPrincipal(
					new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, MockUser.Id), })
				)
			};

			return new ControllerContext() { HttpContext = httpContext };
		}
	}
}
