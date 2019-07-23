using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ShoutboxChannelRepository : RepositoryBase<ShoutboxChannel, SociatisEntities>, IShoutboxChannelRepository
    {
        public ShoutboxChannelRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
