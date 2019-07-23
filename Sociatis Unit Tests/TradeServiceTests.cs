using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using Sociatis_Test_Suite.Dummies.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class TradeServiceTests
    {
        private Mock<TradeService> tradeService;
        private Mock<IEquipmentRepository> equipmentRepository = new Mock<IEquipmentRepository>();
        private Mock<IEquipmentService> equipmentService = new Mock<IEquipmentService>();
        private Mock<ITransactionsService> transactionsService = new Mock<ITransactionsService>();
        private Mock<IWalletService> walletService = new Mock<IWalletService>();
        private Mock<ITradeRepository> tradeRepository = new Mock<ITradeRepository>();
        private Mock<IProductService> productService = new Mock<IProductService>();
        private Mock<IRegionService> regionService = new Mock<IRegionService>();
        private Mock<IEmbargoRepository> embargoRepository = new Mock<IEmbargoRepository>();
        private Mock<IWarningService> warningService = new Mock<IWarningService>();
        private Mock<IWalletRepository> walletRepository = new Mock<IWalletRepository>();
        public TradeServiceTests()
        {
            tradeService = new Mock<TradeService>(equipmentRepository.Object, transactionsService.Object, equipmentService.Object, walletService.Object, tradeRepository.Object,
                productService.Object, regionService.Object, embargoRepository.Object, warningService.Object, walletRepository.Object);
            tradeService.CallBase = true;

            equipmentService.Setup(x => x.GetUnusedInventorySpace(It.IsAny<Equipment>())).Returns(1000);

            SingletonInit.Init();
        }

        [TestMethod]
        public void CancelTradeTest()
        {

            var trade = new TradeDummyCreator().CreateRandom(3, 3);

            tradeService.Object.CancelTrade(trade, trade.Source);

            Assert.IsTrue(trade.Is(TradeStatusEnum.Cancel));
            Assert.AreEqual(0, trade.TradeMoneys.Count);
            Assert.AreEqual(0, trade.TradeProducts.Count);
        }

        [TestMethod]
        public void SourceCanCancelTradeTest()
        {

            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanCancelTrade(trade.Source, trade).isSuccess);
        }

        [TestMethod]
        public void DestinationCanCancelTradeTest()
        {

            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanCancelTrade(trade.Destination, trade).isSuccess);
        }

        [TestMethod]
        public void AlienCannotCancelTradeTest()
        {

            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            var alien = new EntityDummyCreator().Create();

            Assert.IsTrue(tradeService.Object.CanCancelTrade(alien, trade).IsError);
        }

        [TestMethod]
        public void CannotCancelCanceledTradeTest()
        {

            var trade = new TradeDummyCreator()
                .SetStatus(TradeStatusEnum.Cancel)
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanCancelTrade(trade.Source, trade).IsError);
        }



        [TestMethod]
        public void CanStartTradeCitizenTest()
        {
            var country = new CountryDummyCreator().Create();
            var source = new CitizenDummyCreator().SetCountry(country).Create().Entity;
            var destination = new CitizenDummyCreator().SetCountry(country).Create().Entity;

            Assert.IsTrue(tradeService.Object.CanStartTrade(source, destination).isSuccess);

        }

        [TestMethod]
        public void CanAcceptTradeTest()
        {
            var trade = new TradeDummyCreator().CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanAcceptTrade(trade.Source, trade).isSuccess);
            Assert.IsTrue(tradeService.Object.CanAcceptTrade(trade.Destination, trade).isSuccess);
        }

        [TestMethod]
        public void CanNotAcceptTradeWhenDestinationAcceptedTest()
        {
            var trade = new TradeDummyCreator()
                .AcceptTrade(isSource: false)
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanAcceptTrade(trade.Source, trade).isSuccess);
            Assert.IsTrue(tradeService.Object.CanAcceptTrade(trade.Destination, trade).IsError);
        }

        [TestMethod]
        public void CanNotAcceptTradeWhenSourceAcceptedTest()
        {
            var trade = new TradeDummyCreator()
                .AcceptTrade(isSource: true)
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanAcceptTrade(trade.Source, trade).IsError);
            Assert.IsTrue(tradeService.Object.CanAcceptTrade(trade.Destination, trade).isSuccess);
        }

        [TestMethod]
        public void CanFinishWhenAcceptedTest()
        {
            var trade = new TradeDummyCreator()
                .AcceptTrade(isSource: true)
                .AcceptTrade(isSource: false)
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanFinishTrade(trade).isSuccess);
        }

        [TestMethod]
        public void CanFinishWhenAcceptedWhenNoOneAcceptedTest()
        {
            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanFinishTrade(trade).IsError);
        }

        [TestMethod]
        public void CanFinishWhenAcceptedWhenSourceNotAcceptedTest()
        {
            var trade = new TradeDummyCreator()
                .AcceptTrade(isSource: false)
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanFinishTrade(trade).IsError);
        }

        [TestMethod]
        public void CanFinishWhenAcceptedWhenDestinationNotAcceptedTest()
        {
            var trade = new TradeDummyCreator()
                .AcceptTrade(isSource: true)
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.CanFinishTrade(trade).IsError);
        }

        [TestMethod]
        public void ShoudAbortTradeTest()
        {
            var trade = new TradeDummyCreator()
                .SetUpdatedDate(DateTime.Now.Subtract(new TimeSpan(3, 0, 0)))
                .CreateRandom(3, 3);

            Assert.IsTrue(tradeService.Object.ShouldAbortTrade(trade));
        }

        [TestMethod]
        public void ShoudAbortFinishedTradeTest()
        {
            var trade = new TradeDummyCreator()
                .SetStatus(TradeStatusEnum.Success)
                .SetUpdatedDate(DateTime.Now.Subtract(new TimeSpan(3, 0, 0)))
                .CreateRandom(3, 3);

            Assert.IsFalse(tradeService.Object.ShouldAbortTrade(trade));
        }

        [TestMethod]
        public void ShoudNotAbortTradeTest()
        {
            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            Assert.IsFalse(tradeService.Object.ShouldAbortTrade(trade));
        }

        [TestMethod]
        public void GetSecondSideSourceTest()
        {
            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            Assert.AreEqual(trade.Source.EntityID, tradeService.Object.GetSecondSide(trade, trade.Destination).EntityID);
        }
        [TestMethod]
        public void GetSecondSideDestinationTest()
        {
            var trade = new TradeDummyCreator()
                .CreateRandom(3, 3);

            Assert.AreEqual(trade.Destination.EntityID, tradeService.Object.GetSecondSide(trade, trade.Source).EntityID);
        }
    }
}
