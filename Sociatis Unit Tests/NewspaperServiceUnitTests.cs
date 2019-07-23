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
    public class NewspaperServiceUnitTests
    {
        NewspaperService newspaperService;


        public NewspaperServiceUnitTests()
        {
            newspaperService = new NewspaperService(Mock.Of<INewspaperRepository>(), Mock.Of<IEntityService>(), Mock.Of<IEntityRepository>()
                , Mock.Of<IArticleRepository>(), Mock.Of<IUploadService>(), Mock.Of<IWarningService>(), Mock.Of<ITransactionsService>(), Mock.Of<IWalletService>(), Mock.Of<IConfigurationRepository>(),
                Mock.Of<ICitizenService>());

            SingletonInit.Init();
        }

        [TestMethod]
        public void CanCreateNewspaperAsCitizen()
        {
            Entity entity = new CitizenDummyCreator().Create().Entity;

            Assert.IsFalse(newspaperService.CanCreateNewspaper(entity).IsError);
        }

        [TestMethod]
        public void CanCreateNewspaperAsCountry()
        {
            Entity entity = new CountryDummyCreator().Create().Entity;

            Assert.IsFalse(newspaperService.CanCreateNewspaper(entity).IsError);
        }

        [TestMethod]
        public void CanCreateNewspaperAsParty()
        {
            Entity entity = new PartyDummyCreator().Create().Entity;

            Assert.IsFalse(newspaperService.CanCreateNewspaper(entity).IsError);
        }

        [TestMethod]
        public void CanCreateNewspaperAsCompany()
        {
            Entity entity = new CompanyDummyCreator().Create().Entity;

            Assert.IsFalse(newspaperService.CanCreateNewspaper(entity).IsError);
        }

        [TestMethod]
        public void CanCreateNewspaperAsNewspaperError()
        {
            Entity entity = new NewspaperDummyCreator().Create().Entity;

            Assert.IsTrue(newspaperService.CanCreateNewspaper(entity).IsError);
        }
    }
}
