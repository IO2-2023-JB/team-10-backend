using Entities.Data;
using Entities.Models;

namespace Contracts
{
	public interface IUsersRepository : IRepositoryBase<User>
	{
		Task<User> FindUserByEmail(string email);

		UserDto CheckPermissionToGetAccountBalance(string requesterId, UserDto userDTO);
	}
}
