using Entities.Data.Subscription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public class SubscriptionsControllerTests : BaseControllerTests<SubscriptionsController>
	{
		protected override SubscriptionsController GetController()
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();
			var manager = new SubscriptionsManager();
			var controller = new SubscriptionsController(repositoryWrapperMock.Object, mapper, manager);

			return controller;
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17e123")]
		public async Task SubscribeTest_NonExistentUser(string userId)
		{
			var controller = GetController();
			var result = await controller.Subscribe(userId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa03f")]
		[InlineData("64390ed1d3768498801aa04f")]
		public async Task SubscribeTest_UserIsNotCreator(string creatorId)
		{
			var controller = GetController();
			var result = await controller.Subscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f")]
		public async Task SubscribeTest_SelfSubscription(string creatorId)
		{
			var controller = GetController();
			controller.ControllerContext = GetControllerContext(
				"64390ed1d3768498801aa14f",
				Entities.Enums.UserType.Creator,
				"selfSub"
			);

			var result = await controller.Subscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa03f", "64390ed1d3768498801aa14f")]
		[InlineData("64390ed1d3768498801aa04f", "64390ed1d3768498801aa14f")]
		public async Task SubscribeTest_AlreadySubscribed(string subscriberId, string creatorId)
		{
			var controller = GetController();
			controller.ControllerContext = GetControllerContext(
				subscriberId,
				Entities.Enums.UserType.Simple,
				"alreadySubscribed"
			);

			var result = await controller.Subscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa05f", "64390ed1d3768498801aa14f")]
		public async Task SubscribeTest(string subscriberId, string creatorId)
		{
			var controller = GetController();
			controller.ControllerContext = GetControllerContext(
				subscriberId,
				Entities.Enums.UserType.Simple,
				"alreadySubscribed"
			);

			var result = await controller.Subscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}

		[Theory]
		[InlineData("123123123123123123123123")]
		public async Task GetSubscriptionsTest_NonExistentUser(string userId)
		{
			var controller = GetController();
			var result = await controller.GetSubscriptions(userId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa04f")]
		public async Task GetSubscriptionsTest_UserWithSubscriptions(string userId)
		{
			var controller = GetController();
			var result = await controller.GetSubscriptions(userId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
			Assert.IsAssignableFrom<SubscriptionListDto>(result!.Value);
			Assert.IsType<SubscriptionListDto>(result!.Value);
			Assert.NotEmpty((result!.Value as SubscriptionListDto)!.Subscriptions);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa05f")]
		public async Task GetSubscriptionsTest_UserWithoutSubscriptions(string userId)
		{
			var controller = GetController();
			var result = await controller.GetSubscriptions(userId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
			Assert.IsAssignableFrom<SubscriptionListDto>(result!.Value);
			Assert.IsType<SubscriptionListDto>(result!.Value);
			Assert.Empty((result!.Value as SubscriptionListDto)!.Subscriptions);
		}

		[Theory]
		[InlineData("123123123123123123123123")]
		public async Task UnsubscribeTest_NonExistentUser(string creatorId)
		{
			var controller = GetController();
			var result = await controller.Unsubscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa05f", "64390ed1d3768498801aa14f")]
		public async Task UnsubscribeTest_SubscriptionNotFound(string subscriberId, string creatorId)
		{
			var controller = GetController();
			controller.ControllerContext = GetControllerContext(
				subscriberId,
				Entities.Enums.UserType.Simple,
				"user with no subscribed channels"
			);

			var result = await controller.Unsubscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa04f", "64390ed1d3768498801aa14f")]
		public async Task UnsubscribeTest_Success(string subscriberId, string creatorId)
		{
			var controller = GetController();
			controller.ControllerContext = GetControllerContext(
				subscriberId,
				Entities.Enums.UserType.Simple,
				"user with no subscribed channels"
			);

			var result = await controller.Unsubscribe(creatorId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}
	}
}
