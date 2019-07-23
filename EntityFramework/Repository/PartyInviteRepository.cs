using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class PartyInviteRepository : RepositoryBase<PartyInvite, SociatisEntities>, IPartyInviteRepository
    {
        public PartyInviteRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
