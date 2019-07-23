using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class PartyJoinRequestRepository : RepositoryBase<PartyJoinRequest, SociatisEntities>, IPartyJoinRequestRepository
    {
        public PartyJoinRequestRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
