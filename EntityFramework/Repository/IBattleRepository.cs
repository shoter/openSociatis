using Common.EntityFramework;
using Entities.Models.Battles;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IBattleRepository : IRepository<Battle>
    {
        void AddParticipant(BattleParticipant participant);
        IQueryable<AggregatedBattleParticipant> GetParticipantsForBattle(int battleID);
    }
}
