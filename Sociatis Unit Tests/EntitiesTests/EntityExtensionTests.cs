using Entities.enums;
using Entities.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests.EntitiesTests
{
    [TestClass]
    public class EntityExtensionTests
    {
        [TestMethod]
        public void IsTest()
        {
            var citizen = new CitizenDummyCreator().Create();

            Assert.IsTrue(citizen.Entity.Is(EntityTypeEnum.Citizen));
        }
        [TestMethod]
        public void IsMismatchTest()
        {
            var citizen = new CitizenDummyCreator().Create();

            Assert.IsFalse(citizen.Entity.Is(EntityTypeEnum.Company));
        }
        [TestMethod]
        public void IsFewEntitiesTest()
        {
            var citizen = new CitizenDummyCreator().Create();

            Assert.IsTrue(citizen.Entity.Is(EntityTypeEnum.Company, EntityTypeEnum.Citizen, EntityTypeEnum.Newspaper));
        }

    }
}
