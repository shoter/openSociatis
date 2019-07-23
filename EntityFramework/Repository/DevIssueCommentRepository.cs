using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class DevIssueCommentRepository : RepositoryBase<DevIssueComment, SociatisEntities>, IDevIssueCommentRepository
    {
        public DevIssueCommentRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
