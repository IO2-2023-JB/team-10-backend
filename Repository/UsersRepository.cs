using Contracts;
using Entities.Data.User;
using Entities.DatabaseUtils;
using Entities.Models;
using Microsoft.AspNetCore.Http;

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

        public async Task<string> UploadAvatar(User user, IFormFile file)
        {
            var fileName = user.Id;
            using var stream = await _bucket.OpenUploadStreamAsync(fileName, new MongoDB.Driver.GridFS.GridFSUploadOptions()
            {
                Metadata = new MongoDB.Bson.BsonDocument()
                {
                    { "user", user.Id },
                    { "Type", file.ContentType.ToString() }
                }
            });

            var id = stream.Id; // Unique id of the file
            file.CopyTo(stream);
            await stream.CloseAsync();

            return id.ToString();
        }
    }
}
