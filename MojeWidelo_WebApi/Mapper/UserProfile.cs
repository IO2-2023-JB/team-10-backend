using Entities.Data.User;
using Entities.Models;
using MojeWidelo_WebApi.Helpers;

namespace MojeWidelo_WebApi.Mapper
{
	public class UserProfile : AutoMapper.Profile
	{
		public UserProfile()
		{
			CreateMap<User, UserDto>()
				.ForMember(
					user => user.SubscriptionsCount,
					opt =>
						opt.MapFrom(
							user =>
								user.UserType == Entities.Enums.UserType.Creator ? (int?)user.SubscriptionsCount : null
						)
				);
			CreateMap<UpdateUserDto, User>();

			CreateMap<RegisterRequestDto, User>()
				.ForMember(
					user => user.Password,
					opt => opt.MapFrom(register => HashHelper.HashPassword(register.Password))
				);
		}
	}
}
