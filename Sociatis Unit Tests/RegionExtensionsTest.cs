using Entities;
using Entities.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class RegionExtensionsTest
    {
        private RegionDummyCreator regionCreator = new RegionDummyCreator();

        [TestMethod]
        public void GetNeighboursTest()
        {
            var country = new CountryDummyCreator().Create();

            var startRegion = regionCreator.Create(country);

            var region1 = regionCreator.Create(country);
            var region2 = regionCreator.Create(country);

            var passageCreator = new PassageDummyCreator();

            passageCreator.Create(startRegion, region1);
            passageCreator.Create(region1, startRegion);

            List<int> neighboursIDs = new List<int> { region1.ID, region2.ID };

            var neighbours = startRegion.GetNeighbours();

           foreach(var neighbour in neighbours)
            {
                Assert.IsTrue(neighboursIDs.Contains(neighbour.Region.ID));
            }

        }
    }
}
