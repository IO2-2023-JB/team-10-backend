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
				)
				.ForMember(
					user => user.AccountBalance,
					opt =>
						opt.MapFrom(
							user =>
								user.UserType == Entities.Enums.UserType.Creator ? (decimal?)user.AccountBalance : null
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
