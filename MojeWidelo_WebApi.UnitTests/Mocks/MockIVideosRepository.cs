using Contracts;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using Microsoft.VisualBasic;
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
				},
				new VideoMetadata()
				{
					Id = "6465615b2643675169770867",
					Title = "Video title mock 1 - metadata created",
					Description = "Video description mock 1",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "1" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.MetadataRecordCreated,
					UploadDate = DateTime.Parse("2023-05-17T23:20:59.670+00:00"),
					EditDate = DateTime.Parse("2023-05-17T23:21:04.953+00:00"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "6465615b264367516977086A",
					Title = "Video title mock 1 - uploading",
					Description = "Video description mock 1",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "1" },
					Visibility = Entities.Enums.VideoVisibility.Public,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.Uploading,
					UploadDate = DateTime.Parse("2023-05-17T23:20:59.670+00:00"),
					EditDate = DateTime.Parse("2023-05-17T23:21:04.953+00:00"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "6465615b264367516977086B",
					Title = "Video title mock 1 - failed 2 upload",
					Description = "Video description mock 1",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "1" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.FailedToUpload,
					UploadDate = DateTime.Parse("2023-05-17T23:20:59.670+00:00"),
					EditDate = DateTime.Parse("2023-05-17T23:21:04.953+00:00"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "6465615b264367516977086C",
					Title = "Video title mock 1 - uploaded",
					Description = "Video description mock 1",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "1" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.Uploaded,
					UploadDate = DateTime.Parse("2023-05-17T23:20:59.670+00:00"),
					EditDate = DateTime.Parse("2023-05-17T23:21:04.953+00:00"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "6465615b264367516977086D",
					Title = "Video title mock 1 - processing",
					Description = "Video description mock 1",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "1" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.Processing,
					UploadDate = DateTime.Parse("2023-05-17T23:20:59.670+00:00"),
					EditDate = DateTime.Parse("2023-05-17T23:21:04.953+00:00"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "6465615b264367516977086E",
					Title = "Video title mock 1 - failed 2 process",
					Description = "Video description mock 1",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "1" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.FailedToProcess,
					UploadDate = DateTime.Parse("2023-05-17T23:20:59.670+00:00"),
					EditDate = DateTime.Parse("2023-05-17T23:21:04.953+00:00"),
					Duration = ""
				},
				new VideoMetadata()
				{
					Id = "6465177ea074a4809cea03e8",
					Title = "Video title mock 2",
					Description = "Video description mock 2",
					Thumbnail = null,
					Tags = new[] { "video", "tags", "mock", "2" },
					Visibility = Entities.Enums.VideoVisibility.Private,
					AuthorId = "643bfe61ba19d5c39d32b979",
					ViewCount = 0,
					ProcessingProgress = Entities.Enums.ProcessingProgress.Ready,
					UploadDate = DateTime.Parse("2023-05-17T18:05:50.131+00:00"),
					EditDate = DateTime.Parse("2023-05-17T18:18:05.553+00:00"),
					Duration = ""
				}
			};

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.UpdateViewCount(It.IsAny<string>(), It.IsAny<int>()))
				.ReturnsAsync((string id, int value) => collection.FirstOrDefault(o => o.Id == id)!);

			mock.Setup(m => m.GetVideosByUserId(It.IsAny<string>(), It.IsAny<bool>()))
				.ReturnsAsync(
					(string id, bool isAuthor) =>
						collection.Where(x => x.AuthorId == id && (x.Visibility == VideoVisibility.Public || isAuthor))
				);

			mock.Setup(m => m.GetSubscribedVideos(It.IsAny<IEnumerable<string>>()))
				.ReturnsAsync(
					(IEnumerable<string> creatorsIds) =>
						collection.Where(
							x => creatorsIds.Contains(x.AuthorId) && x.Visibility == VideoVisibility.Public
						)
				);

			return mock;
		}
	}
}
