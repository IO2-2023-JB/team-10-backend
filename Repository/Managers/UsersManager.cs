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

		public void AddAvatarUri(Uri location, IEnumerable<UserBaseDto> users)
		{
			foreach (var user in users)
			{
				if (user.AvatarImage != null)
				{
					user.AvatarImage = location.AbsoluteUri + user.AvatarImage;
				}
			}
		}
	}
}
