using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIVideosRepository : MockIRepositoryBase<IVideoRepository, VideoMetadata>
	{
		public static Mock<IVideoRepository> GetMock()
		{
			var collection = new List<VideoMetadata>()
			{
				new VideoMetadata()
				{
					Id = "64623f1db83bfeff70a313ad",
					Title = "Cho'Gath prowokuje Volibeara",
					Description = "legia to parówy a lech mistrz polski",
					Thumbnail = null,
					Tags = new[] { "football", "lech", "legia" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "643bfe61ba19d5c39d32b979",
					ViewCount = 13,
					ProcessingProgress = Entities.Enums.ProcessingProgress.Ready,
					UploadDate = DateTime.Parse("2023-05-15T14:18:05.410Z"),
					EditDate = DateTime.Parse("2023-05-15T14:34:22.723Z"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "64623f1db83bfeff70a313ac",
					Title = "Cho'Gath prowokuje Volibeara",
					Description = "legia to parówy a lech mistrz polski",
					Thumbnail = null,
					Tags = new[] { "football", "lech", "legia" },
					Visibility = Entities.Enums.VideoVisibility.Public,
					AuthorId = "643bfe61ba19d5c39d32b979",
					ViewCount = 13,
					ProcessingProgress = Entities.Enums.ProcessingProgress.Ready,
					UploadDate = DateTime.Parse("2023-05-15T14:18:05.410Z"),
					EditDate = DateTime.Parse("2023-05-15T14:34:22.723Z"),
					Duration = ""
				}
			};

			var mock = GetBaseMock(collection);

			return mock;
		}
	}
}
