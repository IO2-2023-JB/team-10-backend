using Entities.Data.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using MojeWidelo_WebApi.UnitTests.Tests.Controllers;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.ControllersTests
{
	public class UsersControllerTests : BaseControllerTests<UsersController>
	{
		protected override UsersController GetController()
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();
			var usersManager = new UsersManager();
			var controllerContext = GetControllerContext();

			var usersController = new UsersController(repositoryWrapperMock.Object, mapper, usersManager)
			{
				ControllerContext = controllerContext
			};

			return usersController;
		}

		[Fact]
		public async void GetUserByIdFromToken()
		{
			var usersController = GetController();

			var result = await usersController.GetUserById() as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<UserDto>(result?.Value);
			Assert.NotNull(result?.Value as UserDto);
		}

		[Fact]
		public async void GetExistingUserByIdTest()
		{
			var usersController = GetController();

			var result = await usersController.GetUserById("6429a1ee0d48bf254e17eaf7") as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<UserDto>(result?.Value);
			Assert.NotNull(result?.Value as UserDto);
		}

		[Fact]
		public async void GetNonExistingUserByIdTest()
		{
			var usersController = GetController();

			var result = await usersController.GetUserById("1429a1ee0d48bf254e17eaf7") as ObjectResult;

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

			var result = await usersController.RegisterUser(user) as ObjectResult;

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

			var result = await usersController.RegisterUser(user) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status409Conflict, result?.StatusCode);
		}

		[Fact]
		public async void LogInExistingAccount()
		{
			var userController = GetController();
			var loginDto = new LoginDto() { Email = "unit@test.com", Password = "test_password123" };

			var result = await userController.Login(loginDto) as ObjectResult;

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

			var result = await userController.Login(loginDto) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
		}

		[Fact]
		public async void LogInWrongPassword()
		{
			var userController = GetController();
			var loginDto = new LoginDto() { Email = "unit@test.com", Password = "password123" };

			var result = await userController.Login(loginDto) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status401Unauthorized, result?.StatusCode);
		}
	}
}
