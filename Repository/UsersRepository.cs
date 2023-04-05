using Contracts;
using Entities.Data.User;
using Entities.DatabaseUtils;
using Entities.Models;
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
			var result = await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();

			return result;
		}

		public UserDto CheckPermissionToGetAccountBalance(string requesterId, UserDto user)
		{
			if (requesterId != user.Id)
			{
				user.AccountBalance = null;
			}
			return user;
		}

		public async Task<string> UploadAvatar(User user, string file)
		{
			// potrzebna nam ta nazwa wgl? nic z nią nie robimy
			var fileName = "temp";
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
	}
}
