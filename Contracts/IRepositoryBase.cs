using Entities.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Contracts
{
	public interface IRepositoryBase<T>
		where T : MongoDocumentBase
	{
		IMongoCollection<T> _collection { get; }
		IGridFSBucket _bucket { get; }

		Task<IEnumerable<T>> GetAll();
		Task<T> GetById(string id);
		Task<T> Create(T entity);
		Task<T> Update(string id, T entity);
		Task Delete(string id);
	}
}
