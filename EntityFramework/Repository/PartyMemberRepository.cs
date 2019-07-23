using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class PartyMemberRepository : RepositoryBase<PartyMember, SociatisEntities>, IPartyMemberRepository
    {
        public PartyMemberRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
