using Contracts;
using Entities.Data.User;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIVideoRepository
	{
		public static Mock<IVideoRepository> GetMock()
		{
			var mock = new Mock<IVideoRepository>();

			var collection = new List<VideoMetadata>()
			{
				new VideoMetadata()
				{
					Id = "642d38b01826c8963c12dba9",
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					AuthorNickname = "Unit Test User",
					ViewCount = 124,
					ProcessingProgress = Entities.Enums.ProcessingProgress.MetadataRecordCreated,
					UploadDate = DateTime.Now,
					EditDate = DateTime.Now,
					Duration = "0:01",
					Title = "Unit test video's title",
					Description = "Unit test video's description",
					Tags = new List<string>(),
					Visibility = Entities.Enums.VideoVisibility.Private
				},
				new VideoMetadata()
				{
					Id = "642d38b01826c8963c12dba9",
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					AuthorNickname = "Unit Test User",
					ViewCount = 124,
					ProcessingProgress = Entities.Enums.ProcessingProgress.MetadataRecordCreated,
					UploadDate = DateTime.Now,
					EditDate = DateTime.Now,
					Duration = "0:01",
					Title = "Unit test video's title 2",
					Description = "Unit test video's description 2",
					Tags = new List<string>(),
					Visibility = Entities.Enums.VideoVisibility.Public
				}
			};

			mock.Setup(m => m.GetStorageDirectory())
				.Returns(
					() => Environment.GetEnvironmentVariable("MojeWideloStorage", EnvironmentVariableTarget.Machine)
				);

			return mock;
		}
	}
}
