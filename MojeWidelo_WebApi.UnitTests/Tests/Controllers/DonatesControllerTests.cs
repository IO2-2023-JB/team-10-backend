using Entities.Data.Subscription;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public class DonatesControllerTests : BaseControllerTests<DonateController>
	{
		protected override DonateController GetController()
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();
			var controllerContext = GetControllerContext();

			var controller = new DonateController(repositoryWrapperMock.Object, mapper)
			{
				ControllerContext = controllerContext
			};

			return controller;
		}

		protected DonateController GetController(string id)
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();
			var controllerContext = GetControllerContext(id: id);

			var controller = new DonateController(repositoryWrapperMock.Object, mapper)
			{
				ControllerContext = controllerContext
			};

			return controller;
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17eaf7", 10.0)]
		[InlineData("6429a1ee0d48bf254e17eaf7", 100.25)]
		public async Task SendDonation_CantDonateYourself(string id, decimal amount)
		{
			var controller = GetController();
			var result = await controller.SendDonation(id, amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17eff7", 10.0)]
		[InlineData("6429a1ee0d48bf254e17adf7", 100.25)]
		public async Task SendDonation_NonExistingUser(string id, decimal amount)
		{
			var controller = GetController();
			var result = await controller.SendDonation(id, amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("1234a1ee0d48bf254e17eaf7", 10.0)]
		[InlineData("64390ed1d3768498801aa03f", 100.25)]
		public async Task SendDonation_UserIsNotCreator(string id, decimal amount)
		{
			var controller = GetController();
			var result = await controller.SendDonation(id, amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f", 0.0)]
		[InlineData("64390ed1d3768498801aa14f", -100.25)]
		public async Task SendDonation_AmountNotPositive(string id, decimal amount)
		{
			var controller = GetController();
			var result = await controller.SendDonation(id, amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f", 10.0)]
		[InlineData("64390ed1d3768498801aa14f", 100.25)]
		public async Task SendDonation_Success(string id, decimal amount)
		{
			var controller = GetController();
			var result = await controller.SendDonation(id, amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}

		[Theory]
		[InlineData(10.0)]
		[InlineData(100.25)]
		public async Task WithdrawFunds_NotCreator(decimal amount)
		{
			var controller = GetController("1234a1ee0d48bf254e17eaf7");
			var result = await controller.WithdrawFunds(amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result!.StatusCode);
		}

		[Theory]
		[InlineData(0.0)]
		[InlineData(-100.25)]
		public async Task WithdrawFunds_AmountNotPositive(decimal amount)
		{
			var controller = GetController();
			var result = await controller.WithdrawFunds(amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData(1234.0)]
		[InlineData(100.25)]
		public async Task WithdrawFunds_NotEnoughMoneyOnAccount(decimal amount)
		{
			var controller = GetController();
			var result = await controller.WithdrawFunds(amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData(1.0)]
		[InlineData(99.99)]
		public async Task WithdrawFunds_Success(decimal amount)
		{
			var controller = GetController();
			var result = await controller.WithdrawFunds(amount) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}
	}
}
