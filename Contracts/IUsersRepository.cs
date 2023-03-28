﻿using Entities.Data.User;
using Entities.Models;

namespace Contracts
{
	public interface IUsersRepository : IRepositoryBase<User>
	{
		Task<User> FindUserByEmail(string email);

		UserDto CheckPermissionToGetAccountBalance(string requesterId, UserDto userDto);
	        Task<string> UploadAvatar(User user, IFormFile avatarImage);
	}
}
