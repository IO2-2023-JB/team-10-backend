using Contracts;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIRepositoryWrapper
	{
		public static Mock<IRepositoryWrapper> GetMock()
		{
			var mock = new Mock<IRepositoryWrapper>();

			var usersRepoMock = MockIUsersRepository.GetMock();
			var subscriptionsRepoMock = MockISubscriptionsRepository.GetMock();
			var commentsRepoMock = MockICommentsRepository.GetMock();
			var videosRepoMock = MockIVideosRepository.GetMock();
			var reactionsRepoMock = MockIReactionRepository.GetMock();

			mock.Setup(m => m.UsersRepository).Returns(() => usersRepoMock.Object);
			mock.Setup(m => m.SubscriptionsRepository).Returns(() => subscriptionsRepoMock.Object);
			mock.Setup(m => m.CommentRepository).Returns(() => commentsRepoMock.Object);
			mock.Setup(m => m.VideoRepository).Returns(() => videosRepoMock.Object);
			mock.Setup(m => m.ReactionRepository).Returns(() => reactionsRepoMock.Object);

			return mock;
		}
	}
}
