using Entities.Data.Subscription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public class CommentsControllerTests : BaseControllerTests<CommentController>
	{
		protected override CommentController GetController()
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();
			var manager = new CommentManager(mapper);
			var controllerContext = GetControllerContext();

			var controller = new CommentController(repositoryWrapperMock.Object, mapper, manager)
			{
				ControllerContext = controllerContext
			};

			return controller;
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f")]
		[InlineData("64390ed1d3768498801aa04f")]
		public async Task GetComment_NonExistingVideo(string id)
		{
			var controller = GetController();
			var result = await controller.GetComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ad")]
		public async Task GetComment_NoPermissions(string id)
		{
			var controller = GetController();
			var result = await controller.GetComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result!.StatusCode);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async Task GetComment_WithPermissions(string id)
		{
			var controller = GetController();
			var result = await controller.GetComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async Task AddComment(string id)
		{
			var controller = GetController();
			var result = await controller.AddComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async Task DeleteComment_NonExistingComment(string id)
		{
			var controller = GetController();
			var result = await controller.DeleteComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("6453c9bf2c3e1b16d9dd4427")]
		public async Task DeleteComment_NoPermissions(string id)
		{
			var controller = GetController();
			var result = await controller.DeleteComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status401Unauthorized, result!.StatusCode);
		}

		[Theory]
		[InlineData("645181c02c3e1b16d9dd4420")]
		public async Task DeleteComment_HasPermissions(string id)
		{
			var controller = GetController();
			var result = await controller.DeleteComment(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}

		[Theory]
		[InlineData("645181c02c3e1b16d9dd4420")]
		public async Task GetCommentResponse_NoPermissions(string id)
		{
			var controller = GetController();
			var result = await controller.GetCommentResponse(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result!.StatusCode);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async Task GetCommentReponse_NonExistingResponse(string id)
		{
			var controller = GetController();
			var result = await controller.GetCommentResponse(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result!.StatusCode);
		}

		[Theory]
		[InlineData("6453c9bf2c3e1b16d9dd4426")]
		public async Task GetCommentResponse_WithPermissions(string id)
		{
			var controller = GetController();
			var result = await controller.GetCommentResponse(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}

		[Theory]
		[InlineData("6453c9bf2c3e1b16d9dd4426")]
		public async Task AddCommentResponse(string id)
		{
			var controller = GetController();
			var result = await controller.AddCommentResponse(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
		}
	}
}
