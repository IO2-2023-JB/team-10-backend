using Entities.Data.Video;
using Entities.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Options;
using MojeWidelo_WebApi.Controllers;
using Repository.Managers;
using ObjectResult = Microsoft.AspNetCore.Mvc.ObjectResult;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public class VideoControllerTests : BaseControllerTests<VideoController>
	{
		protected override VideoController GetController()
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();

			var location = new Variables() { VideoStorageLocation = "" };
			IOptions<Variables> vars = Options.Create(location);

			var videoManager = new VideoManager(vars);
			var controllerContext = GetControllerContext();

			var videoController = new VideoController(repositoryWrapperMock.Object, mapper, videoManager)
			{
				ControllerContext = controllerContext
			};

			return videoController;
		}

		[Fact]
		public async void UploadVideoMetadataSuccessfully()
		{
			var videoController = GetController();

			var uploadData = new VideoUploadDto()
			{
				Title = "Test title",
				Description = "Test description",
				Thumbnail = null,
				Tags = new[] { "test_tag_1", "test_tag_2", "test_tag_3" },
				Visibility = Entities.Enums.VideoVisibility.Private
			};

			var result = await videoController.UploadVideoMetadata(uploadData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status201Created, result?.StatusCode);
			Assert.IsAssignableFrom<VideoUploadResponseDto>(result?.Value);
			Assert.NotNull(result?.Value as VideoUploadResponseDto);
			// Unable to compare results because of using DateTime.Now
		}

		[Fact]
		public async void UpdateVideoMetadataVideoNotExisting()
		{
			var videoController = GetController();

			var updateData = new VideoUpdateDto()
			{
				Title = "Test update title",
				Description = "Test update description",
				Thumbnail = null,
				Tags = new[] { "test_tag_1_update", "test_tag_2_update", "test_tag_3_update" },
				Visibility = Entities.Enums.VideoVisibility.Public
			};

			var result =
				await videoController.UpdateVideoMetadata("64651cbf1754ecd2e4bc6f86", updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Fact]
		public async void UpdateVideoMetadataUserNotOwner()
		{
			var videoController = GetController();

			var updateData = new VideoUpdateDto()
			{
				Title = "Test update title",
				Description = "Test update description",
				Thumbnail = null,
				Tags = new[] { "test_tag_1_update", "test_tag_2_update", "test_tag_3_update" },
				Visibility = Entities.Enums.VideoVisibility.Public
			};

			var result =
				await videoController.UpdateVideoMetadata("6465177ea074a4809cea03e8", updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do edycji metadanych.", result?.Value);
		}

		[Fact]
		public async void UpdateVideoMetadataSuccessfully()
		{
			var videoController = GetController();

			var updateData = new VideoUpdateDto()
			{
				Title = "Test update title",
				Description = "Test update description",
				Thumbnail = null,
				Tags = new[] { "test_tag_1_update", "test_tag_2_update", "test_tag_3_update" },
				Visibility = Entities.Enums.VideoVisibility.Public
			};

			var result =
				await videoController.UpdateVideoMetadata("6465615b2643675169770867", updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<VideoMetadataDto>(result?.Value);
			Assert.NotNull(result?.Value as VideoMetadataDto);
			// Unable to compare results because of using DateTime.Now
		}

		[Fact]
		public async void GetVideoMetadataByIdVideoNotExisting()
		{
			var videoController = GetController();

			var result = await videoController.GetVideoMetadataById("64651cbf1754ecd2e4bc6f86") as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Fact]
		public async void GetVideoMetadataByIdUserNotOwner()
		{
			var videoController = GetController();

			var result = await videoController.GetVideoMetadataById("6465177ea074a4809cea03e8") as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do dostępu do metadanych.", result?.Value);
		}

		//[Fact]
		//public async void GetVideoMetadataByIdSuccessfully()
		//{
		//	var videoController = GetController();

		//	var result = await videoController.GetVideoMetadataById("6465615b2643675169770867") as ObjectResult;

		//	Assert.NotNull(result);
		//	Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
		//	Assert.IsAssignableFrom<VideoMetadataDto>(result?.Value);
		//	Assert.NotNull(result?.Value as VideoMetadataDto);
		//}
	}
}
