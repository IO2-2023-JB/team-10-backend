using Contracts;
using Entities.Utils;
using Entities.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Repository
{
	public abstract class RepositoryBase<T> : IRepositoryBase<T>
		where T : MongoDocumentBase
	{
		public IMongoCollection<T> _collection { get; }

		public IGridFSBucket _bucket { get; }

		public RepositoryBase(IDatabaseSettings databaseSettings, string collectionName)
		{
			var client = new MongoClient(databaseSettings.ConnectionString);
			var database = client.GetDatabase(databaseSettings.DatabaseName);

			_bucket = new GridFSBucket(database);
			_collection = database.GetCollection<T>(collectionName);
		}

		public async Task<IEnumerable<T>> GetAll()
		{
			return await _collection.Find(x => true).ToListAsync();
		}

		public async Task<T> GetById(string id)
		{
			return await _collection.Find<T>(x => x.Id == id).FirstOrDefaultAsync();
		}

		public async Task<T> Create(T entity)
		{
			await _collection.InsertOneAsync(entity);
			return entity;
		}

		public async Task<T> Update(string id, T entity)
		{
			await _collection.ReplaceOneAsync(x => x.Id == id, entity);
			return entity;
		}

		public async Task Delete(string id)
		{
			await _collection.DeleteOneAsync(x => x.Id == id);
		}
	}
}
