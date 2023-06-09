﻿using Entities.Data.Video;
using Entities.Enums;
using Entities.Utils;
using Microsoft.AspNetCore.Http;
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

			var location = new Variables() { VideoStorageLocation = "", RecommendationPath = "" };
			IOptions<Variables> vars = Options.Create(location);

			var videoManager = new VideoManager(vars);
			var controllerContext = GetControllerContext();

			var videoController = new VideoController(repositoryWrapperMock.Object, mapper, videoManager)
			{
				ControllerContext = controllerContext
			};

			return videoController;
		}

		protected VideoController GetController(string userId)
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();

			var location = new Variables() { VideoStorageLocation = "", RecommendationPath = "" };
			IOptions<Variables> vars = Options.Create(location);

			var videoManager = new VideoManager(vars);
			var controllerContext = GetControllerContext(userId);

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
		}

		[Theory]
		[InlineData("64651cbf1754ecd2e4bc6f86")]
		public async void UpdateVideoMetadataVideoNotExisting(string id)
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

			var result = await videoController.UpdateVideoMetadata(id, updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6465177ea074a4809cea03e8")]
		public async void UpdateVideoMetadataUserNotOwner(string id)
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

			var result = await videoController.UpdateVideoMetadata(id, updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do edycji metadanych.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b2643675169770867")]
		public async void UpdateVideoMetadataSuccessfully(string id)
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

			var result = await videoController.UpdateVideoMetadata(id, updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<VideoMetadataDto>(result?.Value);
			Assert.NotNull(result?.Value as VideoMetadataDto);
		}

		[Theory]
		[InlineData("64651cbf1754ecd2e4bc6f86")]
		public async void GetVideoMetadataByIdVideoNotExisting(string id)
		{
			var videoController = GetController();

			var result = await videoController.GetVideoMetadataById(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6465177ea074a4809cea03e8")]
		public async void GetVideoMetadataByIdUserNotOwner(string id)
		{
			var videoController = GetController();

			var result = await videoController.GetVideoMetadataById(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do dostępu do metadanych.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b2643675169770867")]
		public async void GetVideoMetadataByIdSuccessfullyPrivate(string id)
		{
			var videoController = GetController();

			var result = await videoController.GetVideoMetadataById(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<VideoMetadataDto>(result?.Value);
			Assert.NotNull(result?.Value as VideoMetadataDto);
		}

		[Theory]
		[InlineData("6465615b264367516977086A")]
		public async void GetVideoMetadataByIdSuccessfullyPublic(string id)
		{
			var videoController = GetController();

			var result = await videoController.GetVideoMetadataById(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<VideoMetadataDto>(result?.Value);
			Assert.NotNull(result?.Value as VideoMetadataDto);
		}

		[Theory]
		[InlineData("64651cbf1754ecd2e4bc6f86")]
		public async void UploadVideoVideoNotExisting(string id)
		{
			var videoController = GetController();

			var result = await videoController.UploadVideo(id, new FormFile(null!, 0, 0, null!, null!)) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6465177ea074a4809cea03e8")]
		public async void UploadVideoUserNotOwner(string id)
		{
			var videoController = GetController();

			var result = await videoController.UploadVideo(id, new FormFile(null!, 0, 0, null!, null!)) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do przesłania wideo.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086A")]
		[InlineData("6465615b264367516977086C")]
		[InlineData("6465615b264367516977086D")]
		public async void UploadVideoInvalidStates(string id)
		{
			var videoController = GetController();

			var result = await videoController.UploadVideo(id, new FormFile(null!, 0, 0, null!, null!)) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
		}

		[Theory]
		[InlineData("64651cbf1754ecd2e4bc6f86")]
		public async void StreamVideoVideoNotExisting(string id)
		{
			var videoController = GetController();

			var result = await videoController.StreamVideo("klucz", id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6465177ea074a4809cea03e8")]
		public async void StreamVideoUserNotOwner(string id)
		{
			var videoController = GetController();

			var result = await videoController.StreamVideo("klucz", id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do streamowania wideo.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b2643675169770867")]
		[InlineData("6465615b264367516977086A")]
		[InlineData("6465615b264367516977086B")]
		[InlineData("6465615b264367516977086C")]
		[InlineData("6465615b264367516977086D")]
		[InlineData("6465615b264367516977086E")]
		public async void StreamVideoInvalidStates(string id)
		{
			var videoController = GetController();

			var result = await videoController.StreamVideo("klucz", id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
		}

		[Theory]
		[InlineData("64651cbf1754ecd2e4bc6f86")]
		public async void DeleteVideoVideoNotExisting(string id)
		{
			var videoController = GetController();

			var result = await videoController.DeleteVideo(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6465177ea074a4809cea03e8")]
		public async void DeleteVideoUserNotOwner(string id)
		{
			var videoController = GetController();

			var result = await videoController.DeleteVideo(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do usunięcia wideo.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086A")]
		[InlineData("6465615b264367516977086D")]
		public async void DeleteVideoInvalidStates(string id)
		{
			var videoController = GetController();

			var result = await videoController.DeleteVideo(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
		}

		[Theory]
		[InlineData("64390ed1d37684988022aa13f")]
		public async void GetUsersVideosUserNotExisting(string id)
		{
			var videoController = GetController();

			var result = await videoController.GetUsersVideos(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Użytkownik o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa05f")]
		public async void GetUsersVideosUserNotCreator(string id)
		{
			var videoController = GetController();

			var result = await videoController.GetUsersVideos(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Użytkownik o podanym ID nie jest twórcą.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086B")]
		public async void GetUserReactionPositive(string videoId)
		{
			var videoController = GetController("6465615b264367516977086C");
			var result = await videoController.GetReaction(videoId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.IsAssignableFrom<ReactionResponseDto>(result?.Value);
			Assert.Equal(1, (result?.Value as ReactionResponseDto)!.PositiveCount);
			Assert.Equal(0, (result?.Value as ReactionResponseDto)!.NegativeCount);
			Assert.Equal(ReactionType.Positive, ((ReactionResponseDto)result.Value).CurrentUserReaction);
		}

		[Theory]
		[InlineData("6465615b264367516977086E")]
		public async void GetUserReactionNegative(string videoId)
		{
			var videoController = GetController("6465177ea074a4809cea03e8");
			var result = await videoController.GetReaction(videoId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.IsAssignableFrom<ReactionResponseDto>(result?.Value);
			Assert.Equal(0, (result?.Value as ReactionResponseDto)!.PositiveCount);
			Assert.Equal(1, (result?.Value as ReactionResponseDto)!.NegativeCount);
			Assert.Equal(ReactionType.Negative, ((ReactionResponseDto)result.Value).CurrentUserReaction);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async void GetUserReactionNone(string videoId)
		{
			var videoController = GetController("6465615b2643675169770867");
			var result = await videoController.GetReaction(videoId) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.IsAssignableFrom<ReactionResponseDto>(result?.Value);
			Assert.Equal(0, (result?.Value as ReactionResponseDto)!.PositiveCount);
			Assert.Equal(0, (result?.Value as ReactionResponseDto)!.NegativeCount);
			Assert.Equal(ReactionType.None, ((ReactionResponseDto)result.Value).CurrentUserReaction);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async void AddUserReactionPositive(string videoId)
		{
			var videoController = GetController("6465615b2643675169770867");
			var reaction = new ReactionDto();
			reaction.ReactionType = ReactionType.Positive;
			var result = await videoController.AddReaction(videoId, reaction) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Reakcja dodana pomyślnie.", result?.Value);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ac")]
		public async void AddUserReactionNegative(string videoId)
		{
			var videoController = GetController("6465615b2643675169770867");
			var reaction = new ReactionDto();
			reaction.ReactionType = ReactionType.Negative;
			var result = await videoController.AddReaction(videoId, reaction) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Reakcja dodana pomyślnie.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086E")]
		public async void AddUserReactionNegativeToNone(string videoId)
		{
			var videoController = GetController("6465177ea074a4809cea03e8");
			var reaction = new ReactionDto();
			reaction.ReactionType = ReactionType.Negative;
			var result = await videoController.AddReaction(videoId, reaction) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Reakcja dodana pomyślnie.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086E")]
		public async void AddUserReactionNegativeToPositive(string videoId)
		{
			var videoController = GetController("6465177ea074a4809cea03e8");
			var reaction = new ReactionDto();
			reaction.ReactionType = ReactionType.Positive;
			var result = await videoController.AddReaction(videoId, reaction) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Reakcja dodana pomyślnie.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086B")]
		public async void AddUserReactionPositiveToNone(string videoId)
		{
			var videoController = GetController("6465615b264367516977086C");
			var reaction = new ReactionDto();
			reaction.ReactionType = ReactionType.Positive;
			var result = await videoController.AddReaction(videoId, reaction) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Reakcja dodana pomyślnie.", result?.Value);
		}

		[Theory]
		[InlineData("6465615b264367516977086B")]
		public async void AddUserReactionPositiveToNegative(string videoId)
		{
			var videoController = GetController("6465615b264367516977086C");
			var reaction = new ReactionDto();
			reaction.ReactionType = ReactionType.Negative;
			var result = await videoController.AddReaction(videoId, reaction) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Reakcja dodana pomyślnie.", result?.Value);
		}
	}
}
