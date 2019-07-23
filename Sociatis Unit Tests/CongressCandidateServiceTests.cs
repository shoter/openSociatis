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
    public class CongressCandidateServiceTests
    {
        CongressCandidateService congressCandidateService;

        public CongressCandidateServiceTests()
        {
            congressCandidateService = new CongressCandidateService(Mock.Of<ICongressCandidateVotingRepository>(), Mock.Of<ICountryRepository>(),
                Mock.Of<ICongressmenRepository>(), Mock.Of<ICitizenService>());
        }

        [TestMethod]
        public void GetColdForCadency_normal_test()
        {
            int length = Constants.CongressCadenceDefaultLength;

            Assert.AreEqual(Constants.CongressCadenceMedalGold, congressCandidateService.GetGoldForCadency(length));

        }

        [TestMethod]
        public void GetColdForCadency_diffrentLength_test()
        {
            for (int i = 5; i < 60; ++i)
            {
                double x = (double)i / Constants.CongressCadenceDefaultLength;

                Assert.AreEqual(Constants.CongressCadenceMedalGold * x, congressCandidateService.GetGoldForCadency(i), 0.02);
            }

        }
    }
}
