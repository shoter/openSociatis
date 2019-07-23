using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CongressVotingReservedMoneyRepository : RepositoryBase<CongressVotingReservedMoney, SociatisEntities>, ICongressVotingReservedMoneyRepository
    {
        public CongressVotingReservedMoneyRepository(SociatisEntities context) : base(context)
        {

            
        }

        public List<CongressVotingReservedMoney> GetReservedMoneyForVoting(int votingID)
        {
            return Where(m => m.CongressVotingID == votingID)
                .ToList();
        }
    }
}
