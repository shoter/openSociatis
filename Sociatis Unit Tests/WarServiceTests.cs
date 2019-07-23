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

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class WarServiceTests : TestsBase
    {
        private Mock<IWarRepository> warRepository = new Mock<IWarRepository>();
        private Mock<ITransactionsService> transactionService = new Mock<ITransactionsService>();
        private Mock<WarService> mockWarService;
        private WarService warService => mockWarService.Object;


        public WarServiceTests()
        {
            mockWarService = new Mock<WarService>(warRepository.Object, Mock.Of<Entities.Repository.IWalletRepository>(), Mock.Of<ICountryRepository>(), transactionService.Object, Mock.Of<IWarningService>(),
                Mock.Of<ICitizenRepository>(), Mock.Of<ICitizenService>(), Mock.Of<IPopupService>(), Mock.Of<IWalletService>(), Mock.Of<IBattleRepository>());
            mockWarService.CallBase = true;

            transactionService.Setup(x => x.PayForResistanceBattle(It.IsAny<Citizen>(), It.IsAny<Region>(), It.IsAny<IWarService>()));

            SingletonInit.Init();
        }

        [TestMethod]
        public void TestWarEndPeriod()
        {
            var creator = new WarDummyCreator();
            var battle = creator.AddBattle(true, 100, 10, 20);
            battle.StartDay = 20;
            battle.Active = false;
            var war = creator.Create();

            warRepository.Setup(x => x.GetAllActiveWars()).Returns(new List<War>() { war });

            warService.ProcessDayChange(51);
            Assert.IsFalse(battle.Active);
        }

        [TestMethod]
        public void TestWarNoEnd()
        {
            var creator = new WarDummyCreator();
            var battle = creator.AddBattle(true, 100, 10, 20);
            battle.StartDay = 21;
            battle.Active = false;
            var war = creator.Create();

            warRepository.Setup(x => x.GetAllActiveWars()).Returns(new List<War>() { war });

            warService.ProcessDayChange(51);
            Assert.IsFalse(battle.Active);
        }

        [TestMethod]
        public void StartRessistanceBattle_activeWar_test()
        {
            var war = new WarDummyCreator()
                .MakeRessistance()
                .Create();

            var battle = new BattleDummyCreator()
                .Create(war, false);

            var battleService = new Mock<IBattleService>();

            battleService.Setup(x => x.CreateBattle(It.IsAny<War>(), It.IsAny<int>(), It.IsAny<WarSideEnum>()))
                .Returns(battle);

            mockWarService.Setup(x => x.GetActiveRessistanceWar(It.IsAny<Country>(), It.IsAny<int>()))
                .Returns(war);

            var newBattle = warService.StartRessistanceBattle(new CitizenDummyCreator().SetCountry(war.Attacker).Create(), war.Defender.Regions.First(), battleService.Object);

            Assert.AreEqual(battle, newBattle);

            battleService.Verify(x => x.CreateBattle(It.IsAny<War>(), It.IsAny<int>(), It.Is<WarSideEnum>(side => side == WarSideEnum.Attacker)));
        }

    }
}
