using Entities.Data.Comment;
using Entities.Models;
using AutoMapper;

namespace Repository.Managers
{
	public class CommentManager
	{
		private IMapper _mapper;

		public CommentManager(IMapper mapper)
		{
			_mapper = mapper;
		}

		public CommentDto[] CreateCommentArray(List<Comment> comments, HashSet<User> users)
		{
			comments.Sort((x1, x2) => x1.CreationDate.CompareTo(x2.CreationDate));

			var commentsDto = new List<CommentDto>();
			comments.ForEach(x =>
			{
				commentsDto.Add(
					_mapper.Map<Comment, CommentDto>(
						x,
						opt =>
							opt.AfterMap(
								(src, dest) =>
								{
									var user = users.TakeWhile((y) => x.AuthorId == y.Id).First();
									dest.Nickname = user.Nickname;
									dest.AvatarImage = user.AvatarImage;
								}
							)
					)
				);
			});

			return commentsDto.ToArray();
		}
	}
}
