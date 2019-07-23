using Entities;
using Entities.Models.Regions;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using Sociatis_Test_Suite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class CountryPresidentServiceTests
    {
        Mock<CountryPresidentService> mockCountryPresidentService;
        CountryPresidentService countryPresidentSerivce => mockCountryPresidentService.Object;

        Mock<ICountryRepository> countryRepository = new Mock<ICountryRepository>();
        Mock<IPresidentVotingRepository> presidentVotingRepository = new Mock<IPresidentVotingRepository>();
        Mock<IRegionRepository> regionRepository = new Mock<IRegionRepository>();

        public CountryPresidentServiceTests()
        {
            mockCountryPresidentService = new Mock<CountryPresidentService>(countryRepository.Object, presidentVotingRepository.Object, regionRepository.Object);
            mockCountryPresidentService.CallBase = true;

            SingletonInit.Init();
        }


        [TestMethod]
        public void IsPresidentOfTrueTest()
        {
            var country = new CountryDummyCreator().Create();
            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            country.SetPresident(citizen);

            Assert.IsTrue(countryPresidentSerivce.IsPresidentOf(citizen, country));

        }

        [TestMethod]
        public void IsPresidentOfFalseTest()
        {
            var country = new CountryDummyCreator().Create();
            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            Assert.IsFalse(countryPresidentSerivce.IsPresidentOf(citizen, country));
        }

        [TestMethod]
        public void CanManageSpawnNotCitizenTest()
        {
            var country = new CountryDummyCreator().CreateNewRegion().CreateNewRegion().Create();
            var newspaper = new NewspaperDummyCreator().Create();

            Assert.AreEqual("You must be a citizen to do that!", countryPresidentSerivce.CanManageSpawn(country, newspaper.Entity, country.Regions.ElementAt(0), true)?.Errors[0]);
        }

        [TestMethod]
        public void CanManageSpawnLastRegionTrueTest()
        {
            var country = new CountryDummyCreator().CreateNewRegion().Create();
            country.Regions.ElementAt(0).CanSpawn = false;
            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            country.SetPresident(citizen);

            Assert.AreEqual("This is your last region where citizens can spawn. You cannot disable spawn here!", countryPresidentSerivce.CanManageSpawn(country, citizen.Entity, country.Regions.ElementAt(1), false)?.Errors[0]);
        }

        [TestMethod]
        public void CanManageSpawnLastRegionFalseTest()
        {
            var country = new CountryDummyCreator().CreateNewRegion().Create();
            country.Regions.ElementAt(0).CanSpawn = false;
            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            country.SetPresident(citizen);

            Assert.IsTrue(countryPresidentSerivce.CanManageSpawn(country, citizen.Entity, country.Regions.ElementAt(0), false).isSuccess);
        }

        [TestMethod]
        public void CanManageSpawnTest()
        {
            var country = new CountryDummyCreator().CreateNewRegion().CreateNewRegion().Create();
            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            country.SetPresident(citizen);

            Assert.IsTrue(countryPresidentSerivce.CanManageSpawn(country, citizen.Entity, country.Regions.ElementAt(0), false).isSuccess);
        }

        [TestMethod]
        public void GetColdForCadency_normal_test()
        {
            int length = Constants.PresidentCadenceDefaultLength;

            Assert.AreEqual(Constants.PresidentCadenceMedalGold, countryPresidentSerivce.GetGoldForCadency(length));

        }

        [TestMethod]
        public void GetColdForCadency_diffrentLength_test()
        {
            for (int i = 5; i < 60; ++ i)
            {
                double x = (double)i / Constants.PresidentCadenceDefaultLength;

                Assert.AreEqual(Constants.PresidentCadenceMedalGold * x, countryPresidentSerivce.GetGoldForCadency(i), 0.02);
            }

        }
    }
}
