using Entities;
using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.enums;
using WebServices.structs;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class BattleServiceTests : TestsBase
    {
        private Mock<IBattleRepository> battleRepository = new Mock<IBattleRepository>();
        private Mock<IRegionRepository> regionRepository = new Mock<IRegionRepository>();
        private Mock<IWarService> warService = new Mock<IWarService>();
        private Mock<IEquipmentRepository> equipmentRepository = new Mock<IEquipmentRepository>();
        private Mock<IWarningService> warningService = new Mock<IWarningService>();
        private Mock<ICountryRepository> countryRepository = new Mock<ICountryRepository>();
        private Mock<ITransactionsService> transactionService = new Mock<ITransactionsService>();
        private Mock<IRegionService> regionService = new Mock<IRegionService>();


        private BattleService battleService => mockBattleService.Object;
        private Mock<BattleService> mockBattleService = new Mock<BattleService>();
        public BattleServiceTests()
        {
            SingletonInit.Init();
            mockBattleService = new Mock<BattleService>(battleRepository.Object, regionRepository.Object, warService.Object, equipmentRepository.Object, warningService.Object, 
                countryRepository.Object, transactionService.Object, regionService.Object, Mock.Of<IWarRepository>(), Mock.Of<ICitizenRepository>(), Mock.Of<ICitizenService>(), Mock.Of<IEntityRepository>());
            mockBattleService.CallBase = true;


            regionService.Setup(x => x.GetPathBetweenRegions(It.IsAny<Region>(), It.IsAny<Region>()))
                .Returns(new Path() { Distance = 100 });



        }

        [TestMethod]
        public void CreateBattle_asAttacker_test()
        {
            var war = new WarDummyCreator().Create();
            mockBattleService.Setup(x => x.MakeBattleStartTransaction(It.IsAny<War>(), It.IsAny<Country>(), It.IsAny<double>()))
                .Returns(TransactionResult.Success);
            mockBattleService.Setup(x => x.CalculateWallHealth(It.IsAny<Region>())).Returns(123);
            mockBattleService.Setup(x => x.SendMessageAboutAttack(It.IsAny<War>(), It.IsAny<WarSideEnum>(), It.IsAny<Region>()));

            var battle = battleService.CreateBattle(war, war.Defender.Regions.First().ID, WarSideEnum.Attacker);


            Assert.IsTrue(battle.Active);
            Assert.IsTrue(battle.AttackerInitiatedBattle);
            Assert.AreEqual(0, battle.BattleParticipants.Count);
            Assert.AreEqual(war.Defender.Regions.First().ID, battle.RegionID);
            Assert.AreEqual(123, battle.WallHealth);
            Assert.AreEqual(war, battle.War);
        }

        [TestMethod]
        public void CreateBattle_asDefender_test()
        {
            var war = new WarDummyCreator().Create();
            mockBattleService.Setup(x => x.MakeBattleStartTransaction(It.IsAny<War>(), It.IsAny<Country>(), It.IsAny<double>()))
                .Returns(TransactionResult.Success);
            mockBattleService.Setup(x => x.CalculateWallHealth(It.IsAny<Region>())).Returns(123);
            mockBattleService.Setup(x => x.SendMessageAboutAttack(It.IsAny<War>(), It.IsAny<WarSideEnum>(), It.IsAny<Region>()));

            var battle = battleService.CreateBattle(war, war.Attacker.Regions.First().ID, WarSideEnum.Defender);

            Assert.IsTrue(battle.Active);
            Assert.IsFalse(battle.AttackerInitiatedBattle);
            Assert.AreEqual(0, battle.BattleParticipants.Count);
            Assert.AreEqual(war.Attacker.Regions.First().ID, battle.RegionID);
            Assert.AreEqual(123, battle.WallHealth);
            Assert.AreEqual(war, battle.War);
        }

        [TestMethod]
        public void CreateBattle_asAttackerRessistanceWar_test()
        {
            var war = new WarDummyCreator()
                .MakeRessistance()
                .Create();

            mockBattleService.Setup(x => x.MakeBattleStartTransaction(It.IsAny<War>(), It.IsAny<Country>(), It.IsAny<double>()))
                .Returns(TransactionResult.Success);
            mockBattleService.Setup(x => x.CalculateWallHealth(It.IsAny<Region>())).Returns(123);
            mockBattleService.Setup(x => x.SendMessageAboutAttack(It.IsAny<War>(), It.IsAny<WarSideEnum>(), It.IsAny<Region>()));

            var battle = battleService.CreateBattle(war, war.Defender.Regions.First().ID, WarSideEnum.Attacker);


            Assert.IsTrue(battle.Active);
            Assert.IsTrue(battle.AttackerInitiatedBattle);
            Assert.AreEqual(0, battle.BattleParticipants.Count);
            Assert.AreEqual(war.Defender.Regions.First().ID, battle.RegionID);
            Assert.AreEqual(123, battle.WallHealth);
            Assert.AreEqual(war, battle.War);
        }

    }
}
