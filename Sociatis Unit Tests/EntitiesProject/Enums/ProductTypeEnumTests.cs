using Entities.enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests.EntitiesProject.Enums
{
    [TestClass]
    public class ProductTypeEnumTests
    {
        [TestMethod]
        public void IsRaw_simple_test()
        {
            var product = ProductTypeEnum.Oil;

            Assert.IsTrue(product.IsRaw());
        }

        [TestMethod]
        public void IsRaw_notTrue_test()
        {
            var product = ProductTypeEnum.Bread;

            Assert.IsFalse(product.IsRaw());
        }

        [TestMethod]
        public void CanShowQuality_simple_test()
        {

            var product = ProductTypeEnum.Bread;

            Assert.IsTrue(product.CanShowQuality());
        }

        [TestMethod]
        public void CanShowQuality_fuel_test()
        {

            var product = ProductTypeEnum.Fuel;

            Assert.IsFalse(product.CanShowQuality());
        }

        [TestMethod]
        public void CanShowQuality_infrastructure_test()
        {

            var product = ProductTypeEnum.Development;

            Assert.IsFalse(product.CanShowQuality());
        }
    }
}
