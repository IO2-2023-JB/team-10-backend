using MongoDB.Driver;

namespace Contracts
{
	public interface IRepositoryBase<T>
	{
		IMongoCollection<T> _collection { get; }

		Task<IEnumerable<T>> GetAll();
		Task<T> GetById(string id);
		Task<T> Create(T entity);
		Task<T> Update(string id, T entity);
		Task Delete(string id);
	}
}
