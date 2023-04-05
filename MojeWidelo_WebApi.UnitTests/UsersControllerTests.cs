using AutoMapper;
using Entities.Data.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using MojeWidelo_WebApi.Mapper;
using MojeWidelo_WebApi.UnitTests.Mocks;
using System.Security.Claims;

namespace MojeWidelo_WebApi.UnitTests
{
	public class UsersControllerTests
	{
		//private readonly string _bearerToken = MockJwtToken.GenerateJwtToken();

		public static IMapper GetMapper()
		{
			var mappingProfile = new MappingProfile();
			var configuration = new MapperConfiguration(cfs => cfs.AddProfile(mappingProfile));
			return new AutoMapper.Mapper(configuration);
		}

		[Fact]
		public async void GetUserByIdFromToken()
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();
			var mapper = GetMapper();

			var httpContext = new DefaultHttpContext()
			{
				User = new System.Security.Claims.ClaimsPrincipal(
					new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, MockUser.Id), })
				)
			};

			var usersController = new UsersController(repositoryWrapperMock.Object, mapper)
			{
				ControllerContext = new ControllerContext() { HttpContext = httpContext }
			};

			var result = (await usersController.GetUserById()) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
			Assert.IsAssignableFrom<UserDto>(result.Value);
			Assert.NotNull(result.Value as UserDto);
		}

		[Fact]
		public async void GetExistingUserByIdTest()
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();
			var mapper = GetMapper();

			var httpContext = new DefaultHttpContext()
			{
				User = new System.Security.Claims.ClaimsPrincipal(
					new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, MockUser.Id), })
				)
			};

			var usersController = new UsersController(repositoryWrapperMock.Object, mapper)
			{
				ControllerContext = new ControllerContext() { HttpContext = httpContext }
			};
			var result = (await usersController.GetUserById("6429a1ee0d48bf254e17eaf7")) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
			Assert.IsAssignableFrom<UserDto>(result.Value);
			Assert.NotNull(result.Value as UserDto);
		}

		[Fact]
		public async void GetNonExistingUserByIdTest()
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();
			var mapper = GetMapper();
			var usersController = new UsersController(repositoryWrapperMock.Object, mapper);

			var result = (await usersController.GetUserById("1429a1ee0d48bf254e17eaf7")) as NotFoundResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
		}
	}
}
