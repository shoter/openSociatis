using Common;
using Entities.enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using SociatisCommon.Errors.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.structs.Market;

namespace Sociatis_Unit_Tests.Traders
{
    [TestClass]
    public class CitizenTraderTests
    {
        private readonly Mock<IEquipmentService> equipmentService = new Mock<IEquipmentService>();
        private readonly MarketOfferDummyCreator offerCreator;

        public CitizenTraderTests()
        {
            SingletonInit.Init();

            offerCreator = new MarketOfferDummyCreator();
        }

        [TestMethod]
        public void CanBuy_ProductsNotForCitizen_CannotBuy()
        {
            ProductTypeEnum[] forCitizen = { ProductTypeEnum.Bread, ProductTypeEnum.Weapon, ProductTypeEnum.MovingTicket, ProductTypeEnum.Tea, ProductTypeEnum.House };

            var citizen = new CitizenDummyCreator().Create();
            var citizenTrader = new CitizenTrader(citizen.Entity, equipmentService.Object);


            foreach (var productType in Enums.ToArray<ProductTypeEnum>())
            {
                if (forCitizen.Contains(productType))
                    continue;

                var mockTrader = new Mock<ITrader>();
                mockTrader.SetupGet(x => x.TraderType).Returns(TraderTypeEnum.Shop);

                var offer = offerCreator
                    .SetProduct(productType)
                    .Create();

                var result = citizenTrader.CanBuy(offer, mockTrader.Object);

                Assert.IsTrue(result.Is(TraderErrors.YouCannotBuyThat));
            }
        }

        [TestMethod]
        public void CanBuy_NotGoodItem_CannotBuy()
        {
            equipmentService.Setup(x => x.GetAllowedProductsForEntity(It.IsAny<EntityTypeEnum>())).Returns(new List<ProductTypeEnum>() { ProductTypeEnum.Bread });
            var citizen = new CitizenDummyCreator().Create();
            var citizenTrader = new CitizenTrader(citizen.Entity, equipmentService.Object);


            var mockTrader = new Mock<ITrader>();
            mockTrader.SetupGet(x => x.TraderType).Returns(TraderTypeEnum.Shop);

            var offer = offerCreator
                .SetProduct(ProductTypeEnum.House)
                .Create();

            var result = citizenTrader.CanBuy(offer, mockTrader.Object);

            Assert.IsTrue(result.Is(TraderErrors.YouCannotHaveThat));
        }

        [TestMethod]
        public void CanBuy_SellerNotShop_CannotBuy()
        {
            equipmentService.Setup(x => x.GetAllowedProductsForEntity(It.IsAny<EntityTypeEnum>())).Returns(new List<ProductTypeEnum>() { ProductTypeEnum.Bread });
            var citizen = new CitizenDummyCreator().Create();
            var citizenTrader = new CitizenTrader(citizen.Entity, equipmentService.Object);


            var mockTrader = new Mock<ITrader>();
            mockTrader.SetupGet(x => x.TraderType).Returns(TraderTypeEnum.Hotel);

            var offer = offerCreator
                .SetProduct(ProductTypeEnum.Bread)
                .Create();

            var result = citizenTrader.CanBuy(offer, mockTrader.Object);

            Assert.IsTrue(result.Is(TraderErrors.YouCannotBuyThat));
        }

        [TestMethod]
        public void CanBuy_OfferNotInCountry_CannotBuy()
        {
            equipmentService.Setup(x => x.GetAllowedProductsForEntity(It.IsAny<EntityTypeEnum>())).Returns(new List<ProductTypeEnum>() { ProductTypeEnum.Bread });
            var citizen = new CitizenDummyCreator().Create();
            var citizenTrader = new CitizenTrader(citizen.Entity, equipmentService.Object);


            var mockTrader = new Mock<ITrader>();
            mockTrader.SetupGet(x => x.TraderType).Returns(TraderTypeEnum.Shop);

            var offer = offerCreator
                .SetProduct(ProductTypeEnum.Bread)
                .SetCountry(new CountryDummyCreator().Create())
                .Create();

            var result = citizenTrader.CanBuy(offer, mockTrader.Object);

            Assert.IsTrue(result.Is(TraderErrors.NotSelledInYourCountry));
        }

        [TestMethod]
        public void CanBuy_OfferNotInRegion_CannotBuy()
        {
            equipmentService.Setup(x => x.GetAllowedProductsForEntity(It.IsAny<EntityTypeEnum>())).Returns(new List<ProductTypeEnum>() { ProductTypeEnum.Bread });
            var country = new CountryDummyCreator()
                .CreateNewRegion()
                .CreateNewRegion()
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();
            var citizenTrader = new CitizenTrader(citizen.Entity, equipmentService.Object);


            var mockTrader = new Mock<ITrader>();
            mockTrader.SetupGet(x => x.TraderType).Returns(TraderTypeEnum.Shop);

            var offer = offerCreator
                .SetProduct(ProductTypeEnum.Bread)
                .SetRegion(country.Regions.ElementAt(1))
                .Create();

            var result = citizenTrader.CanBuy(offer, mockTrader.Object);

            Assert.IsTrue(result.Is(TraderErrors.NotSelledInYourRegion));
        }

        [TestMethod]
        public void CanBuy_EverythingOk_Buy()
        {
            equipmentService.Setup(x => x.GetAllowedProductsForEntity(It.IsAny<EntityTypeEnum>())).Returns(new List<ProductTypeEnum>() { ProductTypeEnum.Bread });
            var country = new CountryDummyCreator()
                .CreateNewRegion()
                .CreateNewRegion()
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();
            var citizenTrader = new CitizenTrader(citizen.Entity, equipmentService.Object);


            var mockTrader = new Mock<ITrader>();
            mockTrader.SetupGet(x => x.TraderType).Returns(TraderTypeEnum.Shop);
            mockTrader.SetupGet(x => x.RegionID).Returns(citizen.RegionID);

            var offer = offerCreator
                .SetProduct(ProductTypeEnum.Bread)
                .Create();

            var result = citizenTrader.CanBuy(offer, mockTrader.Object);

            Assert.IsTrue(result.isSuccess);
        }
    }


}
