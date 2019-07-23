using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class BattleParticipantCreatorTests
    {
        private static Battle CreateTestBattle()
        {
            return new Battle()
            {
                Active = true,
                ID = 1,
                Region = new Region() { ID = 1 },
                War = new War() { ID = 1 },
                WarID = 1,
                RegionID = 1
            };
        }
        [TestMethod]
        public void BattleParticipantCreatorMinMaxDamageTest()
        {
            var battle = CreateTestBattle();
            var creator = new BattleParticipantCreator();
            for (int i = 0; i < 100; ++i) //we are testing rand so we need more iterations
            {
                
                creator.MinDamage = 50;
                creator.MaxDamage = 250;

                var participant = creator.Create(battle, true);

                Assert.IsTrue((double)participant.DamageDealt >= creator.MinDamage);
                Assert.IsTrue((double)participant.DamageDealt <= creator.MaxDamage);
            }
        }

        public void BattleParticipantCreatorSideTest()
        {
            var battle = CreateTestBattle();
            var creator = new BattleParticipantCreator();

            Assert.IsTrue(creator.Create(battle, true).IsAttacker);
            Assert.IsFalse(creator.Create(battle, false).IsAttacker);
        }

        public void BattleParticipantCreatorHasBattleAttached()
        {
            var battle = CreateTestBattle();
            var creator = new BattleParticipantCreator();

            var part = creator.Create(battle, true);

            Assert.IsTrue(part.Battle == battle);
            Assert.IsTrue(part.BattleID == battle.ID);
        }
    }
}
