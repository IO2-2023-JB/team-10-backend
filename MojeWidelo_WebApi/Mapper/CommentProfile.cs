using Entities.Data.Comment;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
	public class CommentProfile : AutoMapper.Profile
	{
		public CommentProfile()
		{
			CreateMap<Comment, CommentDto>();
		}
	}
}
