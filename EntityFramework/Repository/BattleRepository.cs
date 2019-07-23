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
    public class BattleRepository : RepositoryBase<Battle, SociatisEntities>, IBattleRepository
    {
        public BattleRepository(SociatisEntities context) : base(context)
        {
        }

        public void  AddParticipant(BattleParticipant participant)
        {
            context.BattleParticipants.Add(participant);
        }

        public IQueryable<AggregatedBattleParticipant> GetParticipantsForBattle(int battleID)
        {
            return context.BattleParticipants.Where(p => p.BattleID == battleID)
                .GroupBy(p => new { p.CitizenID, p.Citizen.Entity.Name, p.IsAttacker, p.Citizen.Entity.ImgUrl })
                .Select(g => new AggregatedBattleParticipant()
                {
                    ID = g.Key.CitizenID,
                    Name = g.Key.Name,
                    Damage = g.Sum(x => x.DamageDealt),
                    IsAttacker = g.Key.IsAttacker,
                    ImgUrl = g.Key.ImgUrl
                });
        }

    }
}
