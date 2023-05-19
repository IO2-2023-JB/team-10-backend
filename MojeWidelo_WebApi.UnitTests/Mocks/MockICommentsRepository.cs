using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockICommentsRepository : MockIRepositoryBase<ICommentRepository, Comment>
	{
		public static Mock<ICommentRepository> GetMock()
		{
			var collection = new List<Comment>()
			{
				new Comment()
				{
					Id = "645181c02c3e1b16d9dd4420",
					VideoId = "64623f1db83bfeff70a313ad",
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					Content = "tak tak byczku, +1",
					HasResponses = false,
					CreationDate = DateTime.Parse("2023-05-04T15:05:35.695Z"),
					LastModificationDate = DateTime.Parse("2023-05-04T15:05:35.695Z"),
					OriginCommentId = null
				},
				new Comment()
				{
					Id = "6456a2782c3e1b16d9dd442a",
					VideoId = "64623f1db83bfeff70a313ad",
					AuthorId = "1234a1ee0d48bf254e17eaf7",
					Content = "processingProgress :  \"MetadataRecordCreated\" - chyba nie zostal nigdy wrzucony film",
					HasResponses = false,
					CreationDate = DateTime.Parse("2023-05-06T18:55:45.698Z"),
					LastModificationDate = DateTime.Parse("2023-05-06T18:55:45.698Z"),
					OriginCommentId = "645181c02c3e1b16d9dd4420"
				},
				new Comment()
				{
					Id = "6453c9bf2c3e1b16d9dd4427",
					VideoId = "64623f1db83bfeff70a313ad",
					AuthorId = "64390ed1d3768498801aa03f",
					Content = "to nie jest muzyczna platforma, do usunięcia",
					HasResponses = true,
					CreationDate = DateTime.Parse("2023-05-04T15:05:35.695Z"),
					LastModificationDate = DateTime.Parse("2023-05-04T15:05:35.695Z"),
					OriginCommentId = null
				},
				new Comment()
				{
					Id = "6453c9bf2c3e1b16d9dd4426",
					VideoId = "64623f1db83bfeff70a313ac",
					AuthorId = "64390ed1d3768498801aa03f",
					Content = "heeeeeeeeeeeeeeeee",
					HasResponses = true,
					CreationDate = DateTime.Parse("2023-05-04T15:05:35.695Z"),
					LastModificationDate = DateTime.Parse("2023-05-04T15:05:35.695Z"),
					OriginCommentId = null
				}
			};

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.GetVideoComments(It.IsAny<string>()))
				.ReturnsAsync(
					(string videoId) =>
						collection.Where(c => c.VideoId == videoId && c.OriginCommentId == null).ToList()
				);

			mock.Setup(m => m.GetCommentResponses(It.IsAny<string>()))
				.ReturnsAsync((string commentId) => collection.Where(c => c.OriginCommentId == commentId).ToList());

			mock.Setup(m => m.DeleteCommentResponses(It.IsAny<string>())).Callback(() => { });

			return mock;
		}
	}
}
