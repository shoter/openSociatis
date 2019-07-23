using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class CompanyServiceUnitTests
    {
        Mock<CompanyService> mockCompanyService = new Mock<CompanyService>();
        private CompanyService companyService => mockCompanyService.Object;

        Mock<IJobOfferRepository> jobOfferRepository = new Mock<IJobOfferRepository>();
        Mock<IWarningService> warningService = new Mock<IWarningService>();

        public CompanyServiceUnitTests()
        {
            mockCompanyService = new Mock<CompanyService>(
                Mock.Of<ICitizenService>(), Mock.Of<IConfigurationRepository>(), Mock.Of<IEquipmentRepository>(), Mock.Of<IProductService>(),
                Mock.Of<IProductRepository>(), Mock.Of<ICitizenRepository>(), Mock.Of<ITransactionsService>(), Mock.Of<IJobOfferService>(),
                Mock.Of<ICompanyEmployeeRepository>(), Mock.Of<ICompanyRepository>(), Mock.Of<IEntityService>(), Mock.Of<ICompanyManagerRepository>(), Mock.Of<IRegionRepository>(),
                warningService.Object, jobOfferRepository.Object, Mock.Of<IEquipmentService>(), Mock.Of<IContractService>(), Mock.Of<IWalletService>(), Mock.Of<IPopupService>(),
                Mock.Of<IRegionService>());
            mockCompanyService.CallBase = true;

            SingletonInit.Init();
        }

        [TestMethod]
        public void CanStartWorkAtNoJobOfferErrorTest()
        {
            jobOfferRepository.Setup(x => x.GetById(It.IsAny<int>())).
                Returns<Entities.JobOffer>(null);

            var citizen = new CitizenDummyCreator().Create();

            var result = companyService.CanStartWorkAt(123, citizen);

            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors[0].Contains("exist"));
        }

        [TestMethod]
        public void CanStartWorkAtNullCitizenTest()
        {
            jobOfferRepository.Setup(x => x.GetById(It.IsAny<int>())).
                Returns<Entities.JobOffer>(null);

            var result = companyService.CanStartWorkAt(123, null);

            Assert.IsTrue(result.IsError);
        }

        [TestMethod]
        public void CanStartWorkWorkingSomewhereElseTest()
        {
            jobOfferRepository.Setup(x => x.GetById(It.IsAny<int>())).
                Returns(new JobOffer());

            var citizen = new CitizenDummyCreator().Create();
            citizen.CompanyEmployee = new Entities.CompanyEmployee();

            var result = companyService.CanStartWorkAt(123, citizen);

            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors[0].Contains("company"));
        }

        [TestMethod]
        public void CanStartWorkingTest()
        {
            jobOfferRepository.Setup(x => x.GetById(It.IsAny<int>())).
                Returns(new JobOffer());

            var citizen = new CitizenDummyCreator().Create();

            var result = companyService.CanStartWorkAt(123, citizen);

            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        public void InformCompanyAboutOfferRemovedDueToMinimalWage_Singular_Test()
        {
            var offer = new JobOffer() { CompanyID = 666 };
            companyService.InformCompanyAboutOfferRemovedDueToMinimalWage(offer, 1);

            warningService.Verify(x => x.AddWarning(It.Is<int>(i => offer.CompanyID == i),
                It.Is<string>(msg => msg.Contains("was") || msg.Contains("offer "))), Times.Once);
        }

        [TestMethod]
        public void InformCompanyAboutOfferRemovedDueToMinimalWage_Plural_Test()
        {
            var offer = new JobOffer() { CompanyID = 123 };
            companyService.InformCompanyAboutOfferRemovedDueToMinimalWage(offer, 22);

            warningService.Verify(x => x.AddWarning(It.Is<int>(i => offer.CompanyID == i),
                It.Is<string>(msg => msg.Contains("were") || msg.Contains("offers"))), Times.Once);
        }

        [TestMethod]
        public void InformCompanyAboutOfferRemovedDueToMinimalWage_Plural2_Test()
        {
            var offer = new JobOffer() { CompanyID = 345 };
            companyService.InformCompanyAboutOfferRemovedDueToMinimalWage(offer, 22);

            warningService.Verify(x => x.AddWarning(It.Is<int>(i => offer.CompanyID == i),
                It.Is<string>(msg => msg.Contains("were") || msg.Contains("offers"))), Times.Once);
        }

        [TestMethod]
        public void RemoveJobOffersThatDoesNotMeetMinimalWage_NoRemove_Test()
        {
            jobOfferRepository.Setup(x => x.GetJobOffersWithoutMinimalWage(It.IsAny<decimal>(), It.IsAny<int>())).Returns(new List<JobOffer>());

            companyService.RemoveJobOffersThatDoesNotMeetMinimalWage(1m, 10);
            
            jobOfferRepository.Verify(x => x.Remove(It.IsAny<JobOffer>()), Times.Never);
            jobOfferRepository.Verify(x => x.GetJobOffersWithoutMinimalWage(It.Is<decimal>(d => d == 1m), It.Is<int>(i => i == 10)), Times.Once);
            mockCompanyService.Verify(x => x.InformCompanyAboutOfferRemovedDueToMinimalWage(It.IsAny<JobOffer>(), It.IsAny<int>()), Times.Never);
            jobOfferRepository.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void RemoveJobOffersThatDoesNotMeetMinimalWage_SingleRemove_Test()
        {
            var jo = new JobOffer();
            jobOfferRepository.Setup(x => x.GetJobOffersWithoutMinimalWage(It.IsAny<decimal>(), It.IsAny<int>())).Returns(new List<JobOffer>()
            {
                jo
            });
            // mockCompanyService.Setup(x => x.InformCompanyAboutOfferRemovedDueToMinimalWage(It.IsAny<JobOffer>(), It.IsAny<int>()));

            companyService.RemoveJobOffersThatDoesNotMeetMinimalWage(1m, 10);

            jobOfferRepository.Verify(x => x.Remove(It.Is<JobOffer>(o => o == jo)), Times.Once);
            jobOfferRepository.Verify(x => x.GetJobOffersWithoutMinimalWage(It.Is<decimal>(d => d == 1m), It.Is<int>(i => i == 10)), Times.Once);
            mockCompanyService.Verify(x => x.InformCompanyAboutOfferRemovedDueToMinimalWage(It.Is<JobOffer>(o => o == jo), It.IsAny<int>()), Times.Once);
            jobOfferRepository.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void RemoveJobOffersThatDoesNotMeetMinimalWage_TwiceRemove_Test()
        {
            var jobs = new List<JobOffer>()
            {
                new JobOffer() { CompanyID = 1 },
                new JobOffer() {CompanyID  = 1 }
            };
            jobOfferRepository.Setup(x => x.GetJobOffersWithoutMinimalWage(It.IsAny<decimal>(), It.IsAny<int>())).Returns(jobs);
            // mockCompanyService.Setup(x => x.InformCompanyAboutOfferRemovedDueToMinimalWage(It.IsAny<JobOffer>(), It.IsAny<int>()));

            companyService.RemoveJobOffersThatDoesNotMeetMinimalWage(1m, 10);

            jobOfferRepository.Verify(x => x.Remove(It.Is<JobOffer>(o => o == jobs[0])), Times.Once);
            jobOfferRepository.Verify(x => x.Remove(It.Is<JobOffer>(o => o == jobs[1])), Times.Once);

            jobOfferRepository.Verify(x => x.GetJobOffersWithoutMinimalWage(It.Is<decimal>(d => d == 1m), It.Is<int>(i => i == 10)), Times.Once);

            mockCompanyService.Verify(x => x.InformCompanyAboutOfferRemovedDueToMinimalWage(It.IsAny<JobOffer>(), It.Is<int>(i => i == 2)), Times.Once);
            jobOfferRepository.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}
