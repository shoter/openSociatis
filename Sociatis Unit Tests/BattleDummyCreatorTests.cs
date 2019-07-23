using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class BattleDummyCreatorTests
    {
        [TestMethod]
        private War CreateTestWar()
        {
            var war = new Mock<War>();

            war.SetupGet(w => w.Attacker).Returns(new Country()
            {
                ID = 1
            });

            war.Object.AttackerCountryID = 1;


            war.SetupGet(w => w.Defender).Returns(new Country()
            {
                ID = 2
            });
            war.Object.DefenderCountryID = 2;

            return war.Object;
        }

        [TestMethod]
        public void BattleDummyCreatorProperParticipantsCountTest()
        {
            var war = CreateTestWar();
            var creator = new BattleDummyCreator();

            creator.AttackerBattleParticipants = 5;
            creator.DefenderBattleParticipants = 10;

            var battle = creator.Create(war, true);

            Assert.AreEqual(5, battle.BattleParticipants.Where(p => p.IsAttacker).Count());
            Assert.AreEqual(10, battle.BattleParticipants.Where(p => p.IsAttacker == false).Count());
            Assert.AreEqual(15, battle.BattleParticipants.Count());
        }

        [TestMethod]
        public void BattleDummyCreatorDamageDealtTest()
        {
            var war = CreateTestWar();
            var creator = new BattleDummyCreator();

            creator.AttackerBattleParticipants = 1;
            creator.SetWallHealth(0);

            var battle = creator.Create(war, true);

            Assert.IsTrue(battle.WallHealth < 0);
        }
    }
}
