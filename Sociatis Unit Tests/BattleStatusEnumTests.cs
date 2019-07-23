using Entities.enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class BattleStatusEnumTests
    {
        [TestMethod]
        public void ToHumanReadableAllValuesTest()
        {
            foreach(BattleStatusEnum value in Enum.GetValues(typeof(BattleStatusEnum)))
            {
                value.ToHumanReadable(); 
            }
        }
    }
}
