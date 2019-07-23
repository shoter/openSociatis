using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.Helpers;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class MPPServiceTests
    {
        Mock<MPPService> mppService;

        Mock<ICountryService> countryService = new Mock<ICountryService>();
        Mock<IWalletService> walletService = new Mock<IWalletService>();
        Mock<IWarService> warService = new Mock<IWarService>();
        Mock<IMilitaryProtectionPactOfferRepository> mppOfferRepository = new Mock<IMilitaryProtectionPactOfferRepository>();
        Mock<IMilitaryProtectionPactRepository> mppRepository = new Mock<IMilitaryProtectionPactRepository>();
        Mock<ITransactionsService> transactionService = new Mock<ITransactionsService>();
        Mock<IWarningService> warningService = new Mock<IWarningService>();

        public MPPServiceTests()
        {
            mppService = new Mock<MPPService>(countryService.Object, walletService.Object, warService.Object,
                mppOfferRepository.Object, transactionService.Object, warningService.Object, mppRepository.Object);
            mppService.CallBase = true;

            SingletonInit.Init();
        }

        [TestMethod]
        public void ProcessDayChangeOnlyActiveOldAreProcessed()
        {
            var mppCreator = new MPPDummyCreator();
            GameHelper.CurrentDay = 4;
            var noDelete = mppCreator.SetStartDay(1).SetEndDay(10).Create();
            var toDelete = mppCreator.SetStartDay(3).SetEndDay(5).Create();

            List<MilitaryProtectionPact> mpps = new List<MilitaryProtectionPact>();
            mpps.Add(noDelete);
            mpps.Add(toDelete);


            mppRepository.Setup(x => x.Where(It.IsAny<Expression<Func<MilitaryProtectionPact, bool>>>()))
                .Returns<Expression<Func<MilitaryProtectionPact, bool>>>(expr => mpps.Where(expr.Compile()).AsQueryable());

            mppService.Object.ProcessDayChange(GameHelper.CurrentDay + 1);

            mppService.Verify(x => x.EndMPP(It.IsIn(toDelete)), Times.Once);
            mppService.Verify(x => x.EndMPP(It.IsIn(noDelete)), Times.Never);

        }
    }
}
