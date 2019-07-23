using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class WarDummyCreator : IDummyCreator<War>
    {
        private static UniqueIDGenerator IDGen = new UniqueIDGenerator();

        private CountryDummyCreator countryCreator = new CountryDummyCreator();
        private RegionDummyCreator regionCreator = new RegionDummyCreator();
        private BattleDummyCreator battleCreator = new BattleDummyCreator();

        private War dummy;

        public WarDummyCreator()
        {
            dummy = makeDefault();
        }

        private War makeDefault()
        {
            var war = new War()
            {
                ID = IDGen.UniqueID,
                Attacker = countryCreator.Create(),
                Defender = countryCreator.Create(),
                Battles = new List<Battle>(),
                StartDay = 1,
                EndDay = null,
                CountryInWars = new List<CountryInWar>()
            };
            war.AttackerCountryID = war.Attacker.ID;
            war.DefenderCountryID = war.Defender.ID;

            return war;
        }

        public Battle AddBattle(bool isAttackerDefending, int wallHealth = 0, int attackerParticipants = 0, int defenderParticipants = 0)
        {
            for (int i = 0; i < attackerParticipants; ++i)
                battleCreator.AddBattleParticipant(true);
            for (int i = 0; i < defenderParticipants; ++i)
                battleCreator.AddBattleParticipant(false);

            var battle = battleCreator.Create(dummy, isAttackerDefending);

            dummy.Battles.Add(battle);

            return battle;
        }

        public WarDummyCreator MakeRessistance()
        {
            dummy.IsRessistanceWar = true;
            return this;
        }



        public Battle GetBattle(int index)
        {
            return dummy.Battles.ElementAt(index);
        }

        public War Create()
        {
            var temp = dummy;

            dummy = makeDefault();

            return temp;
        }
    }
}
