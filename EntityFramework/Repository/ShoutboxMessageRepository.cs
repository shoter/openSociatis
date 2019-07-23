using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ShoutboxMessageRepository : RepositoryBase<ShoutboxMessage, SociatisEntities>, IShoutboxMessageRepository
    {
        public ShoutboxMessageRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
