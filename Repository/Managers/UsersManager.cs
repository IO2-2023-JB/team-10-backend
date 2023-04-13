using Entities.Data.User;

namespace Repository.Managers
{
	public class UsersManager
	{
		public UserDto CheckPermissionToGetAccountBalance(string requesterId, UserDto user)
		{
			if (requesterId != user.Id)
			{
				user.AccountBalance = null;
			}
			return user;
		}
	}
}
