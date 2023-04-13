using Contracts;
using Entities.Models;
using Moq;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockIRepositoryBase<T, U>
		where T : class, IRepositoryBase<U>
		where U : MongoDocumentBase
	{
		protected static Mock<T> GetBaseMock(List<U> collection)
		{
			var mock = new Mock<T>();

			mock.Setup(m => m.GetAll()).ReturnsAsync(() => collection);

			mock.Setup(m => m.GetById(It.IsAny<string>()))
				.ReturnsAsync((string id) => collection.FirstOrDefault(o => o.Id == id)!);

			mock.Setup(m => m.Create(It.IsAny<U>())).ReturnsAsync((U data) => data);

			mock.Setup(m => m.Delete(It.IsAny<string>())).Callback(() => { });

			mock.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<U>())).ReturnsAsync((string id, U data) => data);

			return mock;
		}
	}
}
