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
    public class HouseDayChangeProcessorTests
    {
        private Mock<HouseDayChangeProcessor> mockHouseDayChangeProcessor;
        private HouseDayChangeProcessor houseDayChangeProcessor => mockHouseDayChangeProcessor.Object;

        public HouseDayChangeProcessorTests()
        {
            mockHouseDayChangeProcessor = new Mock<HouseDayChangeProcessor>(
                Mock.Of<IHouseFurnitureRepository>(), Mock.Of<IHouseChestService>());

            mockHouseDayChangeProcessor.CallBase = true;
        }

        [TestMethod]
        public void CalculateNeededCM_99Condition_Expected1CM()
        {
            Assert.AreEqual(1,
                houseDayChangeProcessor.CalculateNeededCM(99m));
        }

        [TestMethod]
        public void CalculateNeededCM_99_9Condition_Expected1CM()
        {
            Assert.AreEqual(1,
                houseDayChangeProcessor.CalculateNeededCM(99.9m));
        }

        [TestMethod]
        public void CalculateNeededCM_100Condition_Expected0CM()
        {
            Assert.AreEqual(0,
                houseDayChangeProcessor.CalculateNeededCM(100));
        }

        [TestMethod]
        public void CalculateNeededCM_100_9Condition_Expected0CM()
        {
            Assert.AreEqual(0,
                houseDayChangeProcessor.CalculateNeededCM(100.9m));
        }

        [TestMethod]
        public void CalculateNeededCM_98_2Condition_Expected2CM()
        {
            Assert.AreEqual(2,
                houseDayChangeProcessor.CalculateNeededCM(98.2m));
        }
    }
}
