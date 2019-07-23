using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class BattleDummyCreator
    {
        private UniqueIDGenerator IDGen = new UniqueIDGenerator();

        private Battle battle;

        private BattleParticipantCreator participantCreator = new BattleParticipantCreator();
        private RegionDummyCreator regionDummyCreator = new RegionDummyCreator();

        public int AttackerBattleParticipants { get; set; }
        public int DefenderBattleParticipants { get; set; }

        public BattleDummyCreator()
        {
            battle = makeDefault();
        }

        public BattleParticipant AddBattleParticipant(bool isAttacker)
        {
            var participant = participantCreator.Create(battle, isAttacker);
            battle.BattleParticipants.Add(participant);
            return participant;
        }

        public void SetWallHealth(int wallHealth)
        {
            battle.WallHealth = 0;
        }

        private Battle makeDefault()
        {
            battle = new Battle()
            {
                ID = IDGen.UniqueID,
                Active = true,
                WallHealth = 0,
                StartTime = DateTime.Now,
                StartDay = 1,

            };
            return battle;
        }

        public Battle Create(War war, bool isAttackerDefending)
        {
            for (int i = 0; i < AttackerBattleParticipants; ++i)
                AddBattleParticipant(isAttacker: true);
            for (int i = 0; i < DefenderBattleParticipants; ++i)
                AddBattleParticipant(isAttacker: false);

            battle.War = war;
            battle.WarID = war.ID;

            if (isAttackerDefending)
                battle.Region = regionDummyCreator.Create(war.Attacker);
            else
                battle.Region = regionDummyCreator.Create(war.Defender);

            battle.RegionID = battle.Region.ID;


            var temp = battle;
            battle = makeDefault();
            return temp;
        }
    }
}
