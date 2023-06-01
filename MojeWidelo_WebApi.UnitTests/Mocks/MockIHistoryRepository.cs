using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIHistoryRepository : MockIRepositoryBase<IHistoryRepository, UserHistory>
	{
		public static Mock<IHistoryRepository> GetMock()
		{
			var collection = new List<UserHistory>() { };

			var mock = GetBaseMock(collection);

			mock.Setup(m => m.AddToHistory(It.IsAny<string>(), It.IsAny<string>())).Callback(() => { });

			return mock;
		}
	}
}
