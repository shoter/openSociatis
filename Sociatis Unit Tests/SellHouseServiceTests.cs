using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using SociatisCommon.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class SellHouseServiceTests
    {
        private Mock<SellHouseService> mockSellHouseService;
        private SellHouseService sellHouseService => mockSellHouseService.Object;

        private Mock<IHouseRepository> houseRepository = new Mock<IHouseRepository>();
        private Mock<IWalletService> walletService = new Mock<IWalletService>();

        public SellHouseServiceTests()
        {
            mockSellHouseService = new Mock<SellHouseService>(houseRepository.Object, walletService.Object);
            mockSellHouseService.CallBase = true;

            SingletonInit.Init();
        }

        [TestMethod]
        public void CanBuy_NullHouse_Error()
        {
            var citizen = new CitizenDummyCreator().Create();

            var result = sellHouseService.CanBuy(null, citizen.Entity);

            Assert.IsTrue(result.Is(HouseErrors.HouseNotExist));
        }


        [TestMethod]
        public void CanBuy_Company_CannotBuyError()
        {
            var company = new CompanyDummyCreator().Create();
            var house = new HouseDummyCreator().SetSellOffer().Create();

            var result = sellHouseService.CanBuy(house, company.Entity);

            Assert.IsTrue(result.Is(HouseErrors.OnlyCitizenBuyHouse));
        }

        [TestMethod]
        public void CanBuy_NullCitizen_Error()
        {
            var house = new HouseDummyCreator().SetSellOffer().Create();

            var result = sellHouseService.CanBuy(house, null);

            Assert.IsTrue(result.Is(HouseErrors.EntityNotExist));
        }

        [TestMethod]
        public void CanBuy_AlreadyHasHouse_Error()
        {
            var house = new HouseDummyCreator().SetSellOffer().Create();
            var citizen = new CitizenDummyCreator().Create();

            houseRepository.Setup(x => x.HasHouseInRegion(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            mockSellHouseService.Setup(x => x.HaveEnoughCashToBuy(It.IsAny<House>(), It.IsAny<Entity>())).Returns(true);

            var result = sellHouseService.CanBuy(house, citizen.Entity);
            Assert.IsTrue(result.Is(HouseErrors.AlreadyHaveHouse));
        }

        [TestMethod]
        public void CanBuy_NotEnoughCash_Error()
        {
            var house = new HouseDummyCreator().SetSellOffer().Create();
            var citizen = new CitizenDummyCreator().Create();

            houseRepository.Setup(x => x.HasHouseInRegion(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockSellHouseService.Setup(x => x.HaveEnoughCashToBuy(It.IsAny<House>(), It.IsAny<Entity>())).Returns(false);

            var result = sellHouseService.CanBuy(house, citizen.Entity);
            Assert.IsTrue(result.Is(HouseErrors.NotEnoughCash));
        }

        [TestMethod]
        public void CanBuy_EverythingOk_Success()
        {
            var house = new HouseDummyCreator().SetSellOffer().Create();
            var citizen = new CitizenDummyCreator().Create();

            houseRepository.Setup(x => x.HasHouseInRegion(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockSellHouseService.Setup(x => x.HaveEnoughCashToBuy(It.IsAny<House>(), It.IsAny<Entity>())).Returns(true);

            var result = sellHouseService.CanBuy(house, citizen.Entity);
            Assert.IsTrue(result.isSuccess);
        }

        [TestMethod]
        public void CanBuy_NoSellfOffer_Error()
        {
            var house = new HouseDummyCreator().Create();
            var citizen = new CitizenDummyCreator().Create();

            houseRepository.Setup(x => x.HasHouseInRegion(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            mockSellHouseService.Setup(x => x.HaveEnoughCashToBuy(It.IsAny<House>(), It.IsAny<Entity>())).Returns(true);

            var result = sellHouseService.CanBuy(house, citizen.Entity);
            Assert.IsTrue(result.Is(HouseErrors.NotOnSell));
        }


    }
}
