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
                    Videos = new[] {"6465615b2643675169770867"},
                    CreationDate = DateTime.Parse("2023-05-15T14:18:05.410Z"),
                    EditDate = DateTime.Parse("2023-05-15T14:34:22.723Z")
                }
            };

            var mock = GetBaseMock(collection);

            mock.Setup(m => m.GetPlaylistByUserId(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(
                    (string id, string callerID) => collection
                .Where(x => x.AuthorId == id && (x.Visibility == PlaylistVisibility.Public || x.AuthorId == callerID)));

            mock.Setup(m => m.GetAllVisiblePlaylists(It.IsAny<string>()))
                .ReturnsAsync(
                    (string userId) => collection
                .Where(x => x.Visibility == PlaylistVisibility.Public || x.AuthorId == userId));

            return mock;
        }
    }
}
