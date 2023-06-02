using Entities.Models;
using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests.Tests.Repositories
{
	public class PlaylistRepositoryTests
	{
		[Theory]
		[InlineData("6429a1ee0d48bf254e17eaf7", "6429a1ee0d48bf254e17eaf7")]
		public async Task GetPlaylistByUserIdOwnerTest(string id, string callerId)
		{
			var respository = MockIPlaylistRepository.GetMock().Object;
			var result = await respository.GetPlaylistByUserId(id, callerId);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Playlist>>(result);
			Assert.NotNull(result as IEnumerable<Playlist>);
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17eaf7", "64390ed1d3768498801aa03f")]
		public async Task GetPlaylistByUserIdNotOwnerTest(string id, string callerId)
		{
			var respository = MockIPlaylistRepository.GetMock().Object;
			var result = await respository.GetPlaylistByUserId(id, callerId);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Playlist>>(result);
			Assert.NotNull(result as IEnumerable<Playlist>);
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17eaf7")]
		public async Task GetAllVisiblePlaylistsTest(string id)
		{
			var respository = MockIPlaylistRepository.GetMock().Object;
			var result = await respository.GetAllVisiblePlaylists(id);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Playlist>>(result);
			Assert.NotNull(result as IEnumerable<Playlist>);
		}
	}
}
