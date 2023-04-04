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

			mock.Setup(m => m.UsersRepository).Returns(() => usersRepoMock.Object);

			return mock;
		}
	}
}
