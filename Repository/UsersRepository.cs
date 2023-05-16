using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Repository
{
	public class UsersRepository : RepositoryBase<User>, IUsersRepository
	{
		public UsersRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.UsersCollectionName) { }

		public async Task<User> FindUserByEmail(string email)
		{
			var result = await Collection.Find(x => x.Email == email).FirstOrDefaultAsync();

			return result;
		}

		public async Task<string> UploadAvatar(User user, string file)
		{
			string fileName = "temp";
			int startIdx = file.IndexOf(',');
			byte[] imgByteArray = Convert.FromBase64String(file.Substring(startIdx + 1));

			var id = await _bucket.UploadFromBytesAsync(fileName, imgByteArray);

			return id.ToString();
		}

		public async Task<byte[]> GetAvatarBytes(string id)
		{
			var bytes = await _bucket.DownloadAsBytesAsync(ObjectId.Parse(id));

			return bytes;
		}

		public async Task<IEnumerable<User>> GetUsersByIds(IEnumerable<string> ids)
		{
			return await Collection.Find(user => ids.Contains(user.Id)).ToListAsync();
		}

		public async Task UpdateSubscriptionCount(string id, int value)
		{
			var update = Builders<User>.Update.Inc(u => u.SubscriptionsCount, value);
			await Collection.UpdateOneAsync(user => user.Id == id, update);
		}

		public async Task UpdateAccountBalance(string id, double amount)
		{
			var update = Builders<User>.Update.Inc(u => u.AccountBalance, amount);
			await Collection.UpdateOneAsync(user => user.Id == id, update);
		}
	}
}
