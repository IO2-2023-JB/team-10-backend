using Contracts;
using Entities.DatabaseUtils;
using Entities.Models;

namespace Repository
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IDatabaseSettings databaseSettings)
            : base(databaseSettings, databaseSettings.CommentCollectionName) { }
    }
}
