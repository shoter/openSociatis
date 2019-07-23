using CreateSociatisWorld.Creators;
using Entities;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace SociatisUnitTests.Services
{
    [TestClass]
    public class EntityServiceTests : TestsBase
    {
        private Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        private Mock<IReservedEntityNameRepository> reservedEntityNameRepository = new Mock<IReservedEntityNameRepository>();
        private Mock<ICompanyService> companyService;
        private IEntityService entityService;

        [TestInitialize]
        public void Initialize()
        {
            entityService = new EntityService(entityRepository.Object, reservedEntityNameRepository.Object);
            companyService = mockResolver.GetMock<ICompanyService>();
            
        }

        [TestMethod]
        public void IsNameTakenTest()
        {
            nameTakenInitialize();
            Assert.IsTrue(entityService.IsNameTaken("test"));
            Assert.IsTrue(entityService.IsNameTaken("bardzo fajny"));
            Assert.IsTrue(entityService.IsNameTaken("user"));
            Assert.IsTrue(entityService.IsNameTaken("dupa"));
            Assert.IsTrue(entityService.IsNameTaken("mam mleko"));
            Assert.IsFalse(entityService.IsNameTaken("asdasdasdDASdAS"));
        }

        [TestMethod]
        public void IsNameTakenTrimToLowerTest()
        {
            nameTakenInitialize();
            Assert.IsTrue(entityService.IsNameTaken(" tEsT "));
            Assert.IsTrue(entityService.IsNameTaken("     bArDzo      fAjny                          "));
            Assert.IsTrue(entityService.IsNameTaken("uSEr"));
            Assert.IsTrue(entityService.IsNameTaken("  dupa"));
            Assert.IsTrue(entityService.IsNameTaken("mam      mleko   "));
        }

        private void nameTakenInitialize()
        {
            var names = new List<string>()
            {
                "test",
                "bardzo fajny",
                "user"
            };

            var reservedNames = new List<string>()
            {
                "dupa",
                "mam mleko"
            };

            var entities = new List<Entity>();
            var entityCreator = new EntityCreator();
            foreach (var name in names)
            {
                entityCreator.Set(name, Entities.enums.EntityTypeEnum.Citizen);
                entities.Add(entityCreator.Get());
            }

            var reserved = new List<ReservedEntityName>();
            int id = 0;
            foreach (var name in reservedNames)
                reserved.Add(new ReservedEntityName()
                {
                    ID = id++,
                    Name = name
                });

            entityRepository.Setup(x => x.Any(It.IsAny<Expression<Func<Entity, bool>>>())).
                Returns<Expression<Func<Entity, bool>>>(expression => entities.Any(expression.Compile()));

            reservedEntityNameRepository.Setup(x => x.Any(It.IsAny<Expression<Func<ReservedEntityName, bool>>>()))
                .Returns<Expression<Func<ReservedEntityName, bool>>>(expr => reserved.Any(expr.Compile()));
        }

    }
}
