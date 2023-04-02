using Contracts;
using Entities.DatabaseUtils;
using Entities.Models;
using MongoDB.Driver;

namespace Repository
{
    public class UsersRepository : RepositoryBase<User>, IUsersRepository
    {
        public UsersRepository(IDatabaseSettings databaseSettings) 
            : base(databaseSettings, databaseSettings.UsersCollectionName)
        {
        }

        public async Task<User> FindUserByEmail(string email)
        {
            var result = await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();

            return result;
        }
    }
}
