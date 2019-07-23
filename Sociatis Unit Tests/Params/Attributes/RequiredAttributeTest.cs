using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Params.Attributes;

namespace Sociatis_Unit_Tests.Params.Attributes
{
    [TestClass]
    public class RequiredAttributeTest
    {
        [TestMethod]
        public void RequireNullTest()
        {
            var attr = new RequiredAttribute();

            object something = null;

            Assert.IsFalse(attr.Validate(something));
        }

        [TestMethod]
        public void RequireNotNullTest()
        {
            var attr = new RequiredAttribute();

            object something = new object();

            Assert.IsTrue(attr.Validate(something));
        }

        [TestMethod]
        public void NotRequiredTest()
        {
            var attr = new RequiredAttribute(false);

            object something = new object();

            Assert.IsTrue(attr.Validate(null));
            Assert.IsTrue(attr.Validate(something));
        }
    }
}
