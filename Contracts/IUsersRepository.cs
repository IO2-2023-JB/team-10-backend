using Entities.Data.User;
using Entities.Models;
using Microsoft.AspNetCore.Http;

namespace Contracts
{
	public interface IUsersRepository : IRepositoryBase<User>
	{
		Task<User> FindUserByEmail(string email);

		UserDto CheckPermissionToGetAccountBalance(string requesterId, UserDto userDto);
		Task<string> UploadAvatar(User user, string avatarImage);

		Task<byte[]> GetAvatarBytes(string id);
	}
}
