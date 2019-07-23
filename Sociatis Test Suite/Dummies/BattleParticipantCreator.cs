using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class BattleParticipantCreator
    {
        public double MaxDamage { get; set; } = 100.0;
        public double MinDamage { get; set; } = 100.0;

        private UniqueIDGenerator IDGen = new UniqueIDGenerator();
        private CitizenDummyCreator citizenCreator = new CitizenDummyCreator();
        private static Random rand = new Random();
        private BattleParticipant participant;

        public BattleParticipantCreator()
        {
            participant = MakeDefault();
        }

        public BattleParticipant MakeDefault()
        {
            participant = new BattleParticipant()
            {
                Citizen = citizenCreator.Create(),
                WeaponQualityUsed = 2
            };

            participant.CitizenID = participant.Citizen.ID;

            return participant;
        }
        public BattleParticipant Create(Battle battle, bool isAttacker)
        {
            participant.Battle = battle;
            participant.BattleID = battle.ID;
            participant.IsAttacker = isAttacker;
            participant.DamageDealt = (decimal)(rand.NextDouble() * (MaxDamage - MinDamage) + MinDamage);

            battle.WallHealth += isAttacker ? -participant.DamageDealt : participant.DamageDealt;

            var temp = participant;
            participant = MakeDefault();
            return temp;

        }
    }
}
