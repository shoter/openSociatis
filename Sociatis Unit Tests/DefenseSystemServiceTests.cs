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
using WebServices.structs;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class DefenseSystemServiceTests : TestsBase
    {
        private DefenseSystemService defenseSystemService => mockDefenseSystemService.Object;
        private Mock<DefenseSystemService> mockDefenseSystemService;
        private Mock<IConstructionRepository> constructionRepository = new Mock<IConstructionRepository>();
        private Mock<IConstructionService> constructionService = new Mock<IConstructionService>();
        private Mock<IWalletService> walletService = new Mock<IWalletService>();
        
        public DefenseSystemServiceTests()
        {
            mockDefenseSystemService = new Mock<DefenseSystemService>(walletService.Object, constructionRepository.Object, transactionScopeProvider.Object);
            SingletonInit.Init();
        }

        [TestMethod]
        public void GetGoldCostForStartingConstruction_variousCostForVariousQualities_test()
        {
            decimal prevCost = defenseSystemService.GetGoldCostForStartingConstruction(1);
            for (int q = 2; q <= 5; ++q)
            {
                decimal cost = defenseSystemService.GetGoldCostForStartingConstruction(q);
                Assert.AreNotEqual(prevCost, cost);
            }
        }

        [TestMethod]
        public void CanBuildDefenseSystem_NotSameCountry_test()
        {
            var country = new CountryDummyCreator().Create();
            var secondCountry = new CountryDummyCreator().Create();

            Assert.AreEqual("Region does not belongs to your country!",
                defenseSystemService.CanBuildDefenseSystem(secondCountry.Regions.First(), country, 5).Errors[0]);
        }

        [TestMethod]
        public void CanBuildDefenseSystem_wrongLessQuality_test()
        {
            var country = new CountryDummyCreator().Create();
            country.Regions.First().DefenseSystemQuality = 2;

            Assert.AreEqual("You cannot construct defense system of this quality here!",
                defenseSystemService.CanBuildDefenseSystem(country.Regions.First(), country, 1).Errors[0]);
        }

        [TestMethod]
        public void CanBuildDefenseSystem_wrongEqualQuality_test()
        {
            var country = new CountryDummyCreator().Create();
            country.Regions.First().DefenseSystemQuality = 2;

            Assert.AreEqual("You cannot construct defense system of this quality here!",
                defenseSystemService.CanBuildDefenseSystem(country.Regions.First(), country, 2).Errors[0]);
        }

        [TestMethod]
        public void CanBuildDefenseSystem_noMoneyTest_test()
        {
            walletService.Setup(x => x.HaveMoney(It.IsAny<int>(), It.IsAny<Money[]>())).Returns(false);
            var country = new CountryDummyCreator().Create();
            country.Regions.First().DefenseSystemQuality = 2;

            Assert.IsTrue(defenseSystemService.CanBuildDefenseSystem(country.Regions.First(), country, 3).Errors[0].StartsWith("Your country does not have"));
        }

        [TestMethod]
        public void CanBuildDefenseSystem_alreadyBuilding_test()
        {
            walletService.Setup(x => x.HaveMoney(It.IsAny<int>(), It.IsAny<Money[]>())).Returns(true);
            constructionRepository.Setup(x => x.AnyConstructionTypeBuildInRegion(It.IsAny<int>(), It.IsAny<ProductTypeEnum>())).Returns(true);

            var country = new CountryDummyCreator().Create();
            country.Regions.First().DefenseSystemQuality = 2;

            Assert.AreEqual("Defense system is already under construction in this region!",
               defenseSystemService.CanBuildDefenseSystem(country.Regions.First(), country, 3).Errors[0]);
        }

        [TestMethod]
        public void CanBuildDefenseSystem_everythingOK_test()
        {
            walletService.Setup(x => x.HaveMoney(It.IsAny<int>(), It.IsAny<Money[]>())).Returns(true);
            constructionRepository.Setup(x => x.AnyConstructionTypeBuildInRegion(It.IsAny<int>(), It.IsAny<ProductTypeEnum>())).Returns(false);

            var country = new CountryDummyCreator().Create();
            country.Regions.First().DefenseSystemQuality = 2;

            Assert.IsTrue(defenseSystemService.CanBuildDefenseSystem(country.Regions.First(), country, 3).isSuccess);
        }
    }
}
