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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.BigParams.MarketOffers;
using WebServices.structs;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class MarketServiceTests
    {
        private Mock<IEquipmentRepository> equipmentRepository = new Mock<IEquipmentRepository>();
        private Mock<IMarketOfferRepository> marketOfferRepository = new Mock<IMarketOfferRepository>();
        private Mock<ICompanyRepository> companyRepository = new Mock<ICompanyRepository>();
        private Mock<IRegionService> regionService = new Mock<IRegionService>();
        private Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        private Mock<ITransactionsService> transactionService = new Mock<ITransactionsService>();
        private Mock<IWalletService> walletService = new Mock<IWalletService>();
        private Mock<IProductTaxRepository> productTaxRepository = new Mock<IProductTaxRepository>();

        private Mock<IProductService> productService = new Mock<IProductService>();

        private IMarketService marketService;
        private MarketOfferDummyCreator offerCreator = new MarketOfferDummyCreator();

        public MarketServiceTests()
        {
            marketService = new MarketService(equipmentRepository.Object, marketOfferRepository.Object, companyRepository.Object,
                regionService.Object, entityRepository.Object, transactionService.Object, walletService.Object, productTaxRepository.Object,
                productService.Object, Mock.Of<IEmbargoRepository>(), Mock.Of<ICountryRepository>(), Mock.Of<IEquipmentService>(),
                Mock.Of<ICompanyFinanceSummaryService>());

            productService.Setup(x => x.GetAllTaxesForProduct(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(new ProductAllTaxes(new List<ProductTax>()) { VAT = 0m });

            SingletonInit.Init();
        }

       /* 
        * This test is done in bad manner. Maybe it should be deleted or done again.
        * [TestMethod]
        public void AddMarketOfferTest()
        {
            MarketOffer marketOffer = null;

            marketOfferRepository.Setup(x => x.Add(It.IsAny<MarketOffer>()))
                .Callback<MarketOffer>(o => marketOffer = o);

            var country = new CountryDummyCreator().Create();

            AddMarketOfferParameters ps = new AddMarketOfferParameters()
            {
                Amount = 123, //expensive bread. PAY THE MONEY
                CompanyID = 5,
                CountryID = country.ID,
                Price = 123.123,
                ProductType = ProductTypeEnum.Bread,
                Quality = 3
            };

            Company company = new Company()
            {
                ID = 5,
                Region = new Region()
                {
                    CountryID = 3
                }
            };

            bool removedGoodItem = false;

            equipmentRepository.Setup(x => x.First(It.IsAny<Expression<Func<Equipment, bool>>>()))
                .Returns<Expression<Func<Equipment,bool>>>(x => new Equipment() { ID = 123 });

            companyRepository.Setup(x => x.GetById(5)).Returns(company);
                

            equipmentRepository.Setup(x => x.RemoveEquipmentItem(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int, int, int>((eqID, productID, quality, amount) =>
                {
                    removedGoodItem = eqID == 123 &&
                    productID == (int)ps.ProductType &&
                    quality == ps.Quality &&
                    ps.Amount == amount;

                });

            

            marketService.AddOffer(ps);

            
            Assert.IsTrue(marketOffer != null);
            Assert.IsTrue(removedGoodItem);
            Assert.AreEqual(marketOffer.Amount, ps.Amount);
            Assert.AreEqual(marketOffer.CompanyID, ps.CompanyID);
            Assert.AreEqual(marketOffer.CountryID, ps.CountryID);
            Assert.AreEqual((double)marketOffer.Price, ps.Price);
            Assert.AreEqual(marketOffer.ProductID, (int)ps.ProductType);
            Assert.AreEqual(marketOffer.Quality, ps.Quality);
        }
        */
        [TestMethod]
        public void CannotUseFuelTest()
        {
            Assert.IsFalse(marketService.CanUseFuel(new CitizenDummyCreator().Create().Entity));
            Assert.IsFalse(marketService.CanUseFuel(new NewspaperDummyCreator().Create().Entity));
            Assert.IsFalse(marketService.CanUseFuel(new CountryDummyCreator().Create().Entity));
            Assert.IsFalse(marketService.CanUseFuel(new PartyDummyCreator().Create().Entity));
        }

        [TestMethod]
        public void CanUseFuel()
        {
            var orgEntity = new EntityDummyCreator().Create();
            orgEntity.EntityTypeID = (int)EntityTypeEnum.Organisation;

            Assert.IsTrue(marketService.CanUseFuel(new CompanyDummyCreator().Create().Entity));
            Assert.IsTrue(marketService.CanUseFuel(orgEntity));
        }

        [TestMethod]
        public void IsEnoughFuelForTradeSimpleTest()
        {
            MarketOffer offer = offerCreator
                .SetAmount(10)
                .Create();

            Assert.IsTrue(marketService.IsEnoughFuelForTrade(fuelInInventory: 10, neededFuel: 10, offer: offer));
        }

        [TestMethod]
        public void IsEnoughFuelForTradeBiggerThanNeededTest()
        {
            MarketOffer offer = offerCreator
                .SetAmount(10)
                .Create();

            Assert.IsTrue(marketService.IsEnoughFuelForTrade(fuelInInventory: 15, neededFuel: 10, offer: offer));
        }

        [TestMethod]
        public void IsEnoughFuelForTradeLessThanNeededTest()
        {
            MarketOffer offer = offerCreator
                .SetAmount(10)
                .Create();

            Assert.IsFalse(marketService.IsEnoughFuelForTrade(fuelInInventory: 5, neededFuel: 10, offer: offer));
        }

        [TestMethod]
        public void IsEnoughFuelForTradeFuelRecompensateTest()
        {
            MarketOffer offer = offerCreator
                .SetAmount(10)
                .SetProduct(ProductTypeEnum.Fuel)
                .Create();

            Assert.IsTrue(marketService.IsEnoughFuelForTrade(fuelInInventory: 2, neededFuel: 10, offer: offer));
        }

        [TestMethod]
        public void IsEnoughFuelForTradeFuelRecompensateNotEnoughTest()
        {
            MarketOffer offer = offerCreator
                .SetAmount(10)
                .SetProduct(ProductTypeEnum.Fuel)
                .Create();

            Assert.IsFalse(marketService.IsEnoughFuelForTrade(fuelInInventory: 2, neededFuel: 13, offer: offer));
        }

        [TestMethod]
        public void IsEnoughFuelForTradeFuelRecompensateJustTest()
        {
            MarketOffer offer = offerCreator
                .SetAmount(10)
                .SetProduct(ProductTypeEnum.Fuel)
                .Create();

            Assert.IsTrue(marketService.IsEnoughFuelForTrade(fuelInInventory: 2, neededFuel: 12, offer: offer));
        }
    }
}
