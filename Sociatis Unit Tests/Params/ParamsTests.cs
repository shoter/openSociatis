using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests.Params
{
    [TestClass]
    public class ParamsTests
    {
        [TestMethod]
        public void AttributeValidateTest()
        {
            var param = new TestParam()
            {
                RequiredValue = 5,
                OptionalValue = null
            };

            Assert.IsTrue(param.IsValid);

            param.RequiredValue = null;
            Assert.IsFalse(param.IsValid);

            param.OptionalValue = 123;
            Assert.IsFalse(param.IsValid);

        }
    }
}
