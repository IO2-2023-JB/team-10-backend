using Entities.Data.Playlist;
using Entities.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MojeWidelo_WebApi.Controllers;
using Repository.Managers;
using ObjectResult = Microsoft.AspNetCore.Mvc.ObjectResult;

namespace MojeWidelo_WebApi.UnitTests.Tests.Controllers
{
	public class PlaylistControllerTests : BaseControllerTests<PlaylistController>
	{
		protected override PlaylistController GetController()
		{
			var repositoryWrapperMock = GetRepositoryWrapperMock();
			var mapper = GetMapper();
			var controllerContext = GetControllerContext();

			var location = new Variables() { VideoStorageLocation = "" };
			IOptions<Variables> vars = Options.Create(location);

			var videoManager = new VideoManager(vars);

			var controller = new PlaylistController(repositoryWrapperMock.Object, mapper, videoManager)
			{
				ControllerContext = controllerContext
			};

			return controller;
		}

		[Fact]
		public async void CreatePlaylistSuccessfullyTest()
		{
			var playlistController = GetController();

			var uploadData = new CreatePlaylistRequestDto()
			{
				Name = "testowa playlista",
				Visibility = Entities.Enums.PlaylistVisibility.Private
			};

			var result = await playlistController.CreatePlaylist(uploadData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status201Created, result?.StatusCode);
			Assert.IsAssignableFrom<CreatePlaylistResponseDto>(result?.Value);
			Assert.NotNull(result?.Value as CreatePlaylistResponseDto);
		}

		[Theory]
		[InlineData("6477b96b143e6a9ccc83360b")]
		public async void EditPlaylistPlaylistNotExistingTest(string id)
		{
			var playlistController = GetController();

			var updateData = new PlaylistEditDto()
			{
				Name = "testowa playlista edytowana",
				Visibility = Entities.Enums.PlaylistVisibility.Public
			};

			var result = await playlistController.EditPlaylist(id, updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Playlista o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba8d143e6a9ccc8336e0")]
		public async void EditPlaylistUserNotOwnerTest(string id)
		{
			var playlistController = GetController();

			var updateData = new PlaylistEditDto()
			{
				Name = "testowa playlista edytowana",
				Visibility = Entities.Enums.PlaylistVisibility.Public
			};

			var result = await playlistController.EditPlaylist(id, updateData) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do edycji playlisty.", result?.Value);
		}

		[Theory]
		[InlineData("6477b96b143e6a9ccc83360b")]
		public async void DeletePlaylistPlaylistNotExistingTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.DeletePlaylist(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Playlista o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba8d143e6a9ccc8336e0")]
		public async void DeletePlaylistUserNotOwnerTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.DeletePlaylist(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do usunięcia playlisty.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba87143e6a9ccc8336df")]
		public async void DeletePlaylistSuccessfullyTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.DeletePlaylist(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Playlista została usunięta pomyślnie.", result?.Value);
		}

		[Theory]
		[InlineData("6477b1df143e6a9ccc833594")]
		public async void GetPlaylistsForUserUserNotExistingTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.GetPlaylistsForUser(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Użytkownik o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17eaf7")]
		public async void GetPlaylistsForUserSuccessfullyTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.GetPlaylistsForUser(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.IsAssignableFrom<IEnumerable<PlaylistBaseDto>>(result?.Value);
			Assert.NotNull(result?.Value as IEnumerable<PlaylistBaseDto>);
		}

		[Theory]
		[InlineData("6477b96b143e6a9ccc83360b")]
		public async void GetVideosInPlaylistPlaylistNotExistingTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.GetVideosInPlaylist(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Playlista o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba8d143e6a9ccc8336e0")]
		public async void GetVideosInPlaylistUserNotOwnerTest(string id)
		{
			var playlistController = GetController();

			var result = await playlistController.GetVideosInPlaylist(id) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do dostępu do playlisty.", result?.Value);
		}

		[Theory]
		[InlineData("6477b96b143e6a9ccc83360b", "64623f1db83bfeff70a313ac")]
		public async void AddVideoToPlaylistPlaylistNotExistingTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.AddVideoToPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Playlista o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba8d143e6a9ccc8336e0", "64623f1db83bfeff70a313ac")]
		public async void AddVideoToPlaylistUserNotOwnerTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.AddVideoToPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do edycji playlisty.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba87143e6a9ccc8336df", "6477bc01143e6a9ccc833722")]
		public async void AddVideoToPlaylistVideoNotExistingTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.AddVideoToPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba87143e6a9ccc8336df", "64623f1db83bfeff70a313ac")]
		public async void AddVideoToPlaylistVideoAlreadyInPlaylistTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.AddVideoToPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID znajduje się już w tej playliście.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba87143e6a9ccc8336df", "6465615b264367516977086A")]
		public async void AddVideoToPlaylistSuccessfullyTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.AddVideoToPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo zostało dodane do playlisty.", result?.Value);
		}

		[Theory]
		[InlineData("6477b96b143e6a9ccc83360b", "64623f1db83bfeff70a313ac")]
		public async void RemoveVideoFromPlaylistPlaylistNotExistingTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.RemoveVideoFromPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Playlista o podanym ID nie istnieje.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba8d143e6a9ccc8336e0", "64623f1db83bfeff70a313ac")]
		public async void RemoveVideoFromPlaylistUserNotOwnerTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.RemoveVideoFromPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Brak uprawnień do edycji playlisty.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba87143e6a9ccc8336df", "6465615b264367516977086A")]
		public async void RemoveVideoFromPlaylistVideoNotInPlaylistTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.RemoveVideoFromPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo o podanym ID nie znajduje się w tej playliście.", result?.Value);
		}

		[Theory]
		[InlineData("6477ba87143e6a9ccc8336df", "64623f1db83bfeff70a313ac")]
		public async void RemoveVideoFromPlaylistSuccessfullyTest(string id, string videoID)
		{
			var playlistController = GetController();

			var result = await playlistController.RemoveVideoFromPlaylist(id, videoID) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
			Assert.NotNull(result?.Value);
			Assert.Equal("Wideo zostało usunięte z playlisty.", result?.Value);
		}
	}
}
