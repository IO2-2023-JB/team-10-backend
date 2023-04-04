using Contracts;
using Entities.Data.User;
using Entities.DatabaseUtils;
using Entities.Models;
using Microsoft.AspNetCore.Http;
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

			var fileName = "temp";
			byte[] imgByteArray = Convert.FromBase64String(file);

			 var id = await _bucket.UploadFromBytesAsync(
				fileName,
				imgByteArray
			);

			return id.ToString();
		}
	}
}
