using Contracts;
using Entities.Enums;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIPlaylistRepository : MockIRepositoryBase<IPlaylistRepository, Playlist>
	{
		public static Mock<IPlaylistRepository> GetMock()
		{
			var collection = new List<Playlist>()
			{
				new Playlist()
				{
					Id = "6477ba87143e6a9ccc8336df",
					Name = "Test playlist 1",
					Visibility = PlaylistVisibility.Private,
					AuthorId = "6429a1ee0d48bf254e17eaf7",
					Videos = new[] { "64623f1db83bfeff70a313ac" },
					CreationDate = DateTime.Parse("2023-05-15T14:18:05.410Z"),
					EditDate = DateTime.Parse("2023-05-15T14:34:22.723Z")
				},
				new Playlist()
				{
					Id = "6477ba8d143e6a9ccc8336e0",
					Name = "Test playlist 2",
					Visibility = PlaylistVisibility.Private,
					AuthorId = "1234a1ee0d48bf254e17eaf7",
					Videos = new[] { "6465615b2643675169770867" },
					CreationDate = DateTime.Parse("2023-05-15T14:18:05.410Z"),
					EditDate = DateTime.Parse("2023-05-15T14:34:22.723Z")
				},
			};

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.GetPlaylistByUserId(It.IsAny<string>(), It.IsAny<User>()))
				.ReturnsAsync(
					(string id, User caller) =>
						collection.Where(
							x =>
								x.AuthorId == id
								&& (
									x.Visibility == PlaylistVisibility.Public
									|| x.AuthorId == caller.Id
									|| caller.UserType == UserType.Administrator
								)
						)
				);

			mock.Setup(m => m.GetAllVisiblePlaylists(It.IsAny<string>()))
				.ReturnsAsync(
					(string userId) =>
						collection.Where(x => x.Visibility == PlaylistVisibility.Public || x.AuthorId == userId)
				);

			return mock;
		}
	}
}
