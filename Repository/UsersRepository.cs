using Contracts;
using Entities.Data.User;
using Entities.Data.Video;
using Entities.Models;
using Entities.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

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

		public async Task SetAvatar(User user, UserBaseDto userDto)
		{
			if (userDto.AvatarImage != null)
			{
				user.AvatarImage = "api/avatar/";
				user.AvatarImage += await UploadImage(userDto.Nickname += " - avatar", userDto.AvatarImage);
			}
		}

		public async Task<byte[]> GetAvatarBytes(string id)
		{
			return await _bucket.DownloadAsBytesAsync(ObjectId.Parse(id));
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

		public async Task UpdateAccountBalance(string id, decimal amount)
		{
			var update = Builders<User>.Update.Inc(u => u.AccountBalance, amount);
			await Collection.UpdateOneAsync(user => user.Id == id, update);
		}
	}
}
