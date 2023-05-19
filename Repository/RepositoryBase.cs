using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Bson;
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

		public async Task<string> UploadImage(string fileName, string file)
		{
			int startIdx = file.IndexOf(',');
			byte[] imgByteArray = Convert.FromBase64String(file.Substring(startIdx + 1));

			startIdx = file.IndexOf(':');
			int endIdx = file.IndexOf(';');

			var id = await _bucket.UploadFromBytesAsync(
				fileName,
				imgByteArray,
				new MongoDB.Driver.GridFS.GridFSUploadOptions
				{
					Metadata = new BsonDocument
					{
						{ "ContentType", file.Substring(startIdx + 1, endIdx - startIdx - 1) }
					}
				}
			);

			return id.ToString();
		}

		public async Task<string> GetContentType(string id)
		{
			var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, ObjectId.Parse(id));

			using var cursor = await _bucket.FindAsync(filter);
			var fileInfo = cursor.ToList().FirstOrDefault();

			if (fileInfo == null)
			{
				throw new ArgumentNullException(null, nameof(fileInfo));
			}

			var contentType = fileInfo.Metadata.GetValue("ContentType").AsString;

			return contentType;
		}
	}
}
