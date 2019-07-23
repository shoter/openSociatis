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
    public class JobOfferServiceTests
    {
        Mock<JobOfferService> jobOfferService;

        [TestMethod]
        public void IsCompliantWithMinimalWage_LowerThanMinimal_Test()
        {

        }
    }
}
