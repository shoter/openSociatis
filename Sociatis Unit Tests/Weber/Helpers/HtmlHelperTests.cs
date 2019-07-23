using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtils.Helpers;

namespace Sociatis_Unit_Tests.Weber.Helpers
{
    [TestClass]
    public class HtmlHelperTests
    {
        [TestMethod]
        public void ToJavascriptArray_singleInt_test()
        {
            int[] array = { 123 };

            Assert.AreEqual("[123]", array.ToJavascriptArray().ToString());
        }

        [TestMethod]
        public void ToJavascriptArray_emptyArray_test()
        {
            object[] array = { };

            Assert.AreEqual("[]", array.ToJavascriptArray().ToString());
        }

        [TestMethod]
        public void ToJavascriptArray_intArray_test()
        {
            int[] array = { 1,2,3,4,5 };

            Assert.AreEqual("[1,2,3,4,5]", array.ToJavascriptArray().ToString());
        }

        [TestMethod]
        public void ToJavascriptArray_membersTest_test()
        {
            int[] array = { 1, 2, 3, 4, 5 };

            Assert.AreEqual("[2,4,6,8,10]", array.ToJavascriptArray(x => x * 2).ToString());
        }

        [TestMethod]
        public void ToJavascriptArray_twoMembers_test()
        {
            int[] array = { 1, 2, 3, 4, 5 };

            Assert.AreEqual("[[2,0],[4,0],[6,0],[8,0],[10,0]]", array.ToJavascriptArray(x => x * 2, x => 0).ToString());
        }
    }
}
