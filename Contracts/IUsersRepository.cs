using Entities.Models;

namespace Contracts
{
    public interface IUsersRepository : IRepositoryBase<User>
    {
        Task<User> FindUserByEmail(string email);
    }
}
