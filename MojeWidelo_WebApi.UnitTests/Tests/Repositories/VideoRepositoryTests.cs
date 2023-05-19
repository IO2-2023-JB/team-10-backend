using Entities.Models;
using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests.Tests.Repositories
{
	public class VideoRepositoryTests
	{
		[Theory]
		[InlineData("6465615b2643675169770867", 1)]
		[InlineData("6465615b264367516977086E", 400)]
		public async void UpdateViewCountTest(string id, int count)
		{
			var videoRepoMock = MockIVideosRepository.GetMock();
			var result = await videoRepoMock.Object.UpdateViewCount(id, count);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<VideoMetadata>(result);
		}

		[Theory]
		[InlineData("6429a1ee0d48bf254e17eaf7", true)]
		[InlineData("6429a1ee0d48bf254e17eaf7", false)]
		public async void GetVideosByUserIdTest(string id, bool isAuthor)
		{
			var videoRepoMock = MockIVideosRepository.GetMock();
			var result = await videoRepoMock.Object.GetVideosByUserId(id, isAuthor);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<VideoMetadata>>(result);
		}

		[Theory]
		[InlineData(
			new string[] { "64623f1db83bfeff70a313ad", "64623f1db83bfeff70a313ac", "6465615b2643675169770867" },
			"643bfe61ba19d5c39d32b979"
		)]
		[InlineData(
			new string[] { "64623f1db83bfeff70a313ad", "64623f1db83bfeff70a313ac", "6465615b2643675169770867" },
			"6429a1ee0d48bf254e17eaf7"
		)]
		public async void GetVideosTest(IEnumerable<string> videosIDs, string userID)
		{
			var videoRepoMock = MockIVideosRepository.GetMock();
			var result = await videoRepoMock.Object.GetVideos(videosIDs, userID);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<VideoMetadata>>(result);
		}

		[Theory]
		[InlineData("643bfe61ba19d5c39d32b979")]
		[InlineData("6429a1ee0d48bf254e17eaf7")]
		public async void GetAllVisibleVideosTest(string id)
		{
			var videoRepoMock = MockIVideosRepository.GetMock();
			var result = await videoRepoMock.Object.GetAllVisibleVideos(id);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<VideoMetadata>>(result);
		}
	}
}
