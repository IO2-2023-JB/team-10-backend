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

		private static UsersController GetController()
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

			return usersController;
		}

		public static IMapper GetMapper()
		{
			var mappingProfile = new MappingProfile();
			var configuration = new MapperConfiguration(cfs => cfs.AddProfile(mappingProfile));
			return new AutoMapper.Mapper(configuration);
		}

		[Fact]
		public async void GetUserByIdFromToken()
		{
			var usersController = GetController();

			var result = (await usersController.GetUserById()) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<UserDto>(result?.Value);
			Assert.NotNull(result?.Value as UserDto);
		}

		[Fact]
		public async void GetExistingUserByIdTest()
		{
			var usersController = GetController();

			var result = (await usersController.GetUserById("6429a1ee0d48bf254e17eaf7")) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<UserDto>(result?.Value);
			Assert.NotNull(result?.Value as UserDto);
		}

		[Fact]
		public async void GetNonExistingUserByIdTest()
		{
			var usersController = GetController();

			var result = (await usersController.GetUserById("1429a1ee0d48bf254e17eaf7")) as NotFoundResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
		}

		[Fact]
		public async void RegisterNewUserSuccessfully()
		{
			var usersController = GetController();

			var user = new RegisterRequestDto()
			{
				Email = "RegisterNewUserSuccessfully@test.com",
				Name = "Test",
				Surname = "Test",
				Nickname = "Test nickname",
				Password = "test_password123",
				UserType = Entities.Enums.UserType.Simple
			};

			var result = (await usersController.RegisterUser(user)) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status201Created, result?.StatusCode);
		}

		[Fact]
		public async void RegisterUserOnExistingEmail()
		{
			var usersController = GetController();

			var user = new RegisterRequestDto()
			{
				Email = "unit@test.com",
				Name = "Test",
				Surname = "Test",
				Nickname = "Test nickname",
				Password = "test_password123",
				UserType = Entities.Enums.UserType.Simple
			};

			var result = (await usersController.RegisterUser(user)) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status409Conflict, result?.StatusCode);
		}

		[Fact]
		public async void LogInExistingAccount()
		{
			var userController = GetController();
			var loginDto = new LoginDto() { Email = "unit@test.com", Password = "test_password123" };

			var result = (await userController.Login(loginDto)) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<LoginResponseDto>(result?.Value);
			Assert.NotNull(result?.Value as LoginResponseDto);
		}

		[Fact]
		public async void LogInNonExistingAccount()
		{
			var userController = GetController();
			var loginDto = new LoginDto() { Email = "notFound@test.com", Password = "test_password123" };

			var result = (await userController.Login(loginDto)) as NotFoundResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
		}

		[Fact]
		public async void LogInWrongPassword()
		{
			var userController = GetController();
			var loginDto = new LoginDto() { Email = "unit@test.com", Password = "password123" };

			var result = (await userController.Login(loginDto)) as UnauthorizedResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status401Unauthorized, result?.StatusCode);
		}
	}
}
