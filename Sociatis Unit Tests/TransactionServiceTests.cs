using Entities.enums;
using Entities.Models.Hospitals;
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
    public class TransactionServiceTests
    {
        private Mock<TransactionsService> mockTransactionsService;
        private TransactionsService transactionsService => mockTransactionsService.Object;

        public TransactionServiceTests()
        {
            mockTransactionsService = new Mock<TransactionsService>(Mock.Of<ITransactionLogRepository>(), Mock.Of<IEntityRepository>(),
                Mock.Of<IWalletService>(), Mock.Of<IConfigurationRepository>());
            mockTransactionsService.CallBase = true;

            mockTransactionsService.Setup(x => x.MakeTransaction(It.IsAny<Transaction>(), It.IsAny<bool>()))
                .Returns(TransactionResult.Success);

            SingletonInit.Init();
        }

        [TestMethod]
        public void PayForHealing_simple_test()
        {
            var hospital = new HospitalDummyCreator().Create();
            var citizen = new CitizenDummyCreator().Create();
            var currency = new CurrencyDummyCreator().Create();
            var healingPrice = new HealingPrice()
            {
                CurrencyID = currency.ID,
                Cost = 12.34m
            };

            transactionsService.PayForHealing(hospital, citizen, healingPrice);

            mockTransactionsService.Verify(x => x.MakeTransaction(It.Is<Transaction>(
                t => t.TransactionType == TransactionTypeEnum.Healing &&
                t.DestinationEntityID == hospital.CompanyID &&
                t.SourceEntityID == citizen.ID &&
                t.Money.Currency == currency &&
                t.Money.Amount == healingPrice.Cost
                ), It.IsAny<bool>()));

        }
    }
}
