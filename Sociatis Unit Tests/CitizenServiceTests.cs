using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class CitizenServiceTests
    {
        public CitizenService CitizenService { get; set; }

        public CitizenServiceTests()
        {
            CitizenService = new CitizenService(Mock.Of<ICountryRepository>(), Mock.Of<Entities.Repository.IWalletRepository>(),
                Mock.Of<ICitizenRepository>(), Mock.Of<IEntityService>(), Mock.Of<IConfigurationRepository>(),
                Mock.Of<ITransactionsService>(), Mock.Of<IWarningService>(), new PopupService(), Mock.Of<IWalletService>(), Mock.Of<IMessageService>(), Mock.Of<IEquipmentService>());
        }

        /* [TestMethod]
         public void OnSoldArticleTests()
         {
             var citizen = new CitizenDummyCreator().Create();
             List<int> receiveMedalFor = new List<int>(){ 300, 500, 800, 1300 };
             bool used = false;
             CitizenService.Setup(x => x.ReceiveSuperJounralist(It.IsAny<Citizen>()))
                 .Callback(() => used = true);

             foreach(var sold in receiveMedalFor)
             {
                 used = false;
                 citizen.SoldArticles = sold - 1;
                 CitizenService.Object.OnSoldArticle(citizen);
                 Assert.IsTrue(used);
             }

         }*/

        [TestMethod]
        public void CalculateExperienceForLevelTest()
        {
            int i = 1;
            Assert.AreEqual(0, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(2, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(5, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(10, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(18, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(30, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(47, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(71, CitizenService.CalculateExperienceForLevel(i++));
            Assert.AreEqual(104, CitizenService.CalculateExperienceForLevel(i++));
        }

        [TestMethod]
        public void CalculateExperienceForNextLevelTest()
        {
            for (int i = 1; i < 20; ++i)
            {
                Assert.AreEqual(CitizenService.CalculateExperienceForLevel(i + 1), CitizenService.CalculateExperienceForNextLevel(i));
            }
        }
    }
}
