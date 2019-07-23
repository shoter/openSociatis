using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using Sociatis_Test_Suite.Dummies.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.Companies;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class CompanyFinanceSummaryServiceTests
    {
        private Mock<CompanyFinanceSummaryService> mockCompanyFinanceSummaryService;
        private CompanyFinanceSummaryService companyFinanceSummaryService => mockCompanyFinanceSummaryService.Object;

        private Mock<ICompanyFinanceSummaryRepository> companyFinanceSummaryRepository = new Mock<ICompanyFinanceSummaryRepository>();
        public CompanyFinanceSummaryServiceTests()
        {
            mockCompanyFinanceSummaryService = new Mock<CompanyFinanceSummaryService>(companyFinanceSummaryRepository.Object);

            SingletonInit.Init();
        }
        [TestMethod]
        public void AddFinances_SingleFinance_Test()
        {
            Company company = new CompanyDummyCreator().Create();
            var summary = new CompanyFinanceSummaryDummyCreator(company).Create();

            var finance = new Mock<ICompanyFinance>();
            finance.SetupGet(x => x.CurrencyID).Returns(summary.CurrencyID);

            companyFinanceSummaryRepository.Setup(x => x.GetOrAdd(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int[]>()))
                .Returns(new List<CompanyFinanceSummary>() { summary });


            companyFinanceSummaryService.AddFinances(company, finance.Object);

            finance.Verify(x => x.Modify(summary), Times.Once);
            companyFinanceSummaryRepository.Verify(x => x.SaveChanges(), Times.Once);

        }
    }
}
