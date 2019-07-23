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
using WebServices.Helpers;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class EmbargoServiceTests
    {
        private Mock<IEmbargoRepository> embargoRepository = new Mock<IEmbargoRepository>();
        private EmbargoService embargoService;

        public EmbargoServiceTests()
        {
            embargoService = new EmbargoService(Mock.Of<ICountryRepository>(), embargoRepository.Object, Mock.Of<IWarningService>(), Mock.Of<Entities.Repository.IWalletRepository>(), Mock.Of<ITransactionsService>());
            SingletonInit.Init();
        }

        [TestMethod]
        public void ProcessDayChangeNoGoldForUpkeepRegion()
        {

            var embargo = new EmbargoDummyCreator().Create();

            var regionCreator = new RegionDummyCreator();

            regionCreator.Create(embargo.CreatorCountry);
            regionCreator.Create(embargo.EmbargoedCountry);

            embargoRepository.Setup(x => x.GetAllActiveEmbargoes()).Returns(new List<Embargo> { embargo });

            embargoService.ProcessDayChange(123);
            Assert.IsFalse(embargo.Active); // not enough money
        }
    }
}
