using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Repository
{
	public abstract class RepositoryBase<T> : IRepositoryBase<T>
		where T : MongoDocumentBase
	{
		public MongoClient Client { get; }

		public IMongoCollection<T> Collection { get; }

		public IGridFSBucket _bucket { get; }

		public RepositoryBase(IDatabaseSettings databaseSettings, string collectionName)
		{
			Client ??= new MongoClient(databaseSettings.ConnectionString);
			var database = Client.GetDatabase(databaseSettings.DatabaseName);

			var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
			ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

			_bucket = new GridFSBucket(database);
			Collection = database.GetCollection<T>(collectionName);
		}

		public async Task<IEnumerable<T>> GetAll()
		{
			return await Collection.Find(x => true).ToListAsync();
		}

		public async Task<T> GetById(string id)
		{
			return await Collection.Find<T>(x => x.Id == id).FirstOrDefaultAsync();
		}

		public async Task<T> Create(T entity)
		{
			await Collection.InsertOneAsync(entity);
			return entity;
		}

		public async Task<T> Update(string id, T entity)
		{
			await Collection.ReplaceOneAsync(x => x.Id == id, entity);
			return entity;
		}

		public async Task Delete(string id)
		{
			await Collection.DeleteOneAsync(x => x.Id == id);
		}
	}
}
