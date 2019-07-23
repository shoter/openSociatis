using Entities;
using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.structs.Params.MonetaryMarket;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class MonetaryMarketServiceTests
    {

        private Mock<IMonetaryOfferRepository> monetaryOfferRepository = new Mock<IMonetaryOfferRepository>();
        private Mock<IMonetaryTransactionRepository> monetaryTransactionRepository = new Mock<IMonetaryTransactionRepository>();
        private Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        private MonetaryMarketService monetaryMarketService;

        public MonetaryMarketServiceTests()
        {
            monetaryMarketService = new MonetaryMarketService(monetaryOfferRepository.Object, monetaryTransactionRepository.Object, Mock.Of<ITransactionsService>(), 
                entityRepository.Object, Mock.Of<ICountryRepository>());
        }

        [TestMethod]
        public void CreateMonetaryOfferTest()
        {
            CreateMonetaryOfferParam pr = new CreateMonetaryOfferParam()
            {
                Amount = 123,
                BuyCurrency = new Entities.Currency() { ID = 5 },
                SellCurency = new Entities.Currency() { ID = 1 },
                OfferType = MonetaryOfferTypeEnum.Buy,
                Rate = 1.3154,
                Seller = new Entities.Entity() { EntityID = 1 }
            };

            var offer = monetaryMarketService.CreateOffer(pr);

            Assert.AreEqual(pr.Amount, offer.Amount);
            Assert.AreEqual(pr.BuyCurrency.ID, offer.BuyCurrencyID);
            Assert.AreEqual(pr.SellCurency.ID, offer.SellCurrencyID);
            Assert.AreEqual((int)pr.OfferType, offer.OfferTypeID);
            Assert.AreEqual(pr.Rate, (double)offer.Rate);
            Assert.AreEqual(pr.Seller.EntityID, offer.SellerID);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateMonetaryOfferAssertTest()
        {
            var pr = new CreateMonetaryOfferParam() { };

            monetaryMarketService.CreateOffer(pr);
        }

      /*  [TestMethod]
        public void AutomaticBuyOffersTest()
        {
            bool shouldBeDeleted = false;

            monetaryOfferRepository.Setup(x => x.Add(It.IsAny<MonetaryOffer>()))
                .Throws(new Exception("Add should not be called. TestOffer should buy other things"));

            var offers = new List<MonetaryOffer>();
            var creator = new MonetaryOfferDummyCreator()
                .SetMonetaryOfferType(MonetaryOfferTypeEnum.Sell);

            MonetaryOffer offerToDelete, offerToReduceAmount;

            offers.Add(offerToDelete = creator.Create(15.0, 10));
            offers.Add(offerToReduceAmount = creator.Create(15.5, 10));
            offers.Add(creator.Create(16.0, 10));
            offers.Add(creator.Create(16.5, 10));

            monetaryOfferRepository.Setup(x => x.Remove(offerToDelete)).Callback(() => shouldBeDeleted = true);
            monetaryOfferRepository.Setup(x => x.Where(It.IsAny<Expression<Func<MonetaryOffer, bool>>>()))
                .Returns<Expression<Func<MonetaryOffer, bool>>>(predicate => offers.Where(predicate.Compile()).AsQueryable());

            CreateMonetaryOfferParam pr = new CreateMonetaryOfferParam()
            {
                Amount = 11,
                BuyCurrency = new Entities.Currency() { ID = offerToDelete.BuyCurrencyID },
                SellCurency = new Entities.Currency() { ID = offerToDelete.SellCurrencyID },
                OfferType = MonetaryOfferTypeEnum.Buy,
                Rate = 15.5,
                Seller = new Entities.Entity() { EntityID = 1 }
            };

            var offer = monetaryMarketService.CreateOffer(pr);

            Assert.AreEqual(null, offer);
            Assert.AreEqual(0, offerToDelete.Amount);
            Assert.AreEqual(9, offerToReduceAmount.Amount);

            if (shouldBeDeleted == false)
                Assert.Fail("Monetary offer was not removed :(");

            
        }*/

        [TestMethod]
        public void CalculateTaxMinimumTest()
        {
            double minimumTax = 1.9;
            Assert.AreEqual(minimumTax,
                monetaryMarketService.CalculateTax(overallPrice: 1, taxRate: 0.01, minimumTax: minimumTax));
        }

        [TestMethod]
        public void CalculateTaxNotMinimumTest()
        {
            double minimumTax = 1.9;
            int amount = 100;
            double rate = 0.2;
            Assert.AreEqual(amount * rate,
                monetaryMarketService.CalculateTax(overallPrice: amount, taxRate: rate, minimumTax: minimumTax));
        }

    }
}
