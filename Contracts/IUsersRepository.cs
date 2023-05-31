using Entities.Data.User;
using Entities.Models;

namespace Contracts
{
	public interface IUsersRepository : IRepositoryBase<User>
	{
		Task<User> FindUserByEmail(string email);
		Task SetAvatar(User user, UserBaseDto userDto);
		Task<byte[]> GetAvatarBytes(string id);
		Task<IEnumerable<User>> GetUsersByIds(IEnumerable<string> ids);
		Task UpdateSubscriptionCount(string id, int value);
		Task UpdateAccountBalance(string id, decimal amount);
		Task<string> GetNicknameFromID(string id);
	}
}
