using Entities.Models;

namespace Contracts
{
	public interface IUsersRepository : IRepositoryBase<User>
	{
		Task<User> FindUserByEmail(string email);

		Task<string> UploadAvatar(User user, string avatarImage);

		Task<byte[]> GetAvatarBytes(string id);

		Task<IEnumerable<User>> GetUsersByIds(IEnumerable<string> ids);
	}
}
