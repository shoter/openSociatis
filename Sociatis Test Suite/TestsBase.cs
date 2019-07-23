using Common.Tests.Transactions;
using Common.Transactions;
using Moq;
using Sociatis_Test_Suite.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using WebServices;

namespace Sociatis_Test_Suite
{
    public class TestsBase
    {
        protected MockDependencyResolver mockResolver;
        protected Mock<ITransactionScopeProvider> transactionScopeProvider = new Mock<ITransactionScopeProvider>();

        public TestsBase()
        {
            transactionScopeProvider.Setup(x => x.CreateTransactionScope())
                .Returns(Mock.Of<ITransactionScope>());
            transactionScopeProvider.Setup(x => x.CreateNewTransactionScope(It.IsAny<IsolationLevel>(), It.IsAny<TimeSpan?>()))
                .Returns(Mock.Of<ITransactionScope>());


            mockResolver = new MockDependencyResolver();
            mockResolver.AddMock(transactionScopeProvider);
            mockResolver.AddMock<IBattleService>();
            mockResolver.AddMock<ICompanyService>();



            DependencyResolver.SetResolver(mockResolver);
        }
    }
}
