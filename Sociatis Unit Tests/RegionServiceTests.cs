using Entities;
using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class RegionServiceTests
    {
        private Mock<IRegionRepository> regionRepository = new Mock<IRegionRepository>();
        private RegionService regionService;

        public RegionServiceTests()
        {
            regionService = new RegionService(regionRepository.Object, Mock.Of<ICountryRepository>(), Mock.Of<IWarningService>(), Mock.Of<ITransactionsService>());
        }


        [TestMethod]
        public void GetBirthableRegions_noNormalSpawn_test()
        {
            regionRepository.Setup(x => x.GetSpawnableRegionsForCountry(It.IsAny<int>())).Returns(new List<Region>().AsQueryable());

            var list = new List<Region>().AsQueryable();

            regionRepository.Setup(x => x.GetCoreRegionsForCountry(It.IsAny<int>())).Returns(list);

            Assert.AreSame(list, regionService.GetBirthableRegions(1234));
        }

        [TestMethod]
        public void GetBirthableRegions_normalSpawn_test()
        {
            var list = new List<Region>() { new Region() }.AsQueryable();
            regionRepository.Setup(x => x.GetSpawnableRegionsForCountry(It.IsAny<int>())).Returns(list);

           

            Assert.AreEqual(list, regionService.GetBirthableRegions(1234));
        }

    }
}
