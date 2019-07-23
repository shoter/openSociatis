using Entities;
using Entities.enums;
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
using WebServices.structs;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class EntityServiceUnitTests : TestsBase
    {
        Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        Mock<IReservedEntityNameRepository> reservedEntityNameRepository = new Mock<IReservedEntityNameRepository>();
        Mock<ICompanyService> companyService;
        IEntityService entityService;


        CitizenDummyCreator citizenCreator = new CitizenDummyCreator();
        CompanyDummyCreator companyCreator = new CompanyDummyCreator();
        PartyDummyCreator partyCreator = new PartyDummyCreator();
        public EntityServiceUnitTests()
        {
            entityService = new EntityService(entityRepository.Object, reservedEntityNameRepository.Object);
            companyService = mockResolver.GetMock<ICompanyService>();
        }

        [TestMethod]
        public void ExistsTest()
        {
            Entity[] entities = new Entity[2];

            entities[0] = new Entity() { Name = "test" };
            entities[1] = new Entity() { Name = "ZbYsZeK" };

            entityRepository.Setup(x => x.Any(It.IsAny<Expression<Func<Entity, bool>>>())).Returns( (Expression<Func<Entity,bool>> expr) => entities.Any(expr.Compile()));

            Assert.IsTrue(entityService.Exists("test"));
            Assert.IsTrue(entityService.Exists("test "));
            Assert.IsTrue(entityService.Exists(" test"));
            Assert.IsTrue(entityService.Exists("Zbyszek"));
            Assert.IsFalse(entityService.Exists("Zbyszek2"));
        }


        [TestMethod]
        public void ErrorOnChangeIntoWrongCitizenAsCitizen()
        {
            citizenCreator.SetName("OurCitizen");
            var ourCitizen = citizenCreator.Create();

            var someoneCitizen = citizenCreator.Create();

            Assert.IsFalse(entityService.CanChangeInto(ourCitizen.Entity, someoneCitizen.Entity, ourCitizen));
        }

        [TestMethod]
        public void CanChangeIntoCitizenAsCompany()
        {
            var ourCompany = companyCreator.Create();
            var ourCitizen = citizenCreator.Create();

            Assert.IsTrue(entityService.CanChangeInto(ourCompany.Entity, ourCitizen.Entity, ourCitizen));
        }

       /* [TestMethod]
        public void CanChangeIntoCompanyAsCitizen()
        {
            var ourCitizen = citizenCreator.Create();
            companyCreator.SetOwner(ourCitizen.Entity);
            var ourCompany = companyCreator.Create();

            Assert.IsTrue(entityService.CanChangeInto(ourCitizen.Entity, ourCompany.Entity, ourCitizen));
        }*/

        [TestMethod]
        public void ErrorOnChangeIntoCompanyAsCompany()
        {
            companyService.Setup(x => x.GetCompanyRights(It.IsAny<Company>(), It.IsAny<Entity>(), It.IsAny<Citizen>()))
                .Returns(new CompanyRights(fullRights: true));

            var ourCitizen = citizenCreator.Create();
            companyCreator.SetOwner(ourCitizen.Entity);
            var ourCompany = companyCreator.Create();
            companyCreator.SetOwner(ourCitizen.Entity);
            var ourCompany2 = companyCreator.Create();

            Assert.IsFalse(entityService.CanChangeInto(ourCompany.Entity, ourCompany2.Entity, ourCitizen));
        }

      /*  [TestMethod]
        public void ErrorOnChangeIntoNotOwnerCompanyAsCitizen()
        {
            var ourCitizen = citizenCreator.Create();
            var company = companyCreator.Create();

            Assert.IsFalse(entityService.CanChangeInto(ourCitizen.Entity, company.Entity, ourCitizen));
        }*/

        [TestMethod]
        public void ErrorOnChangeIntoNotYourPartyAsCitizen()
        {
            var ourCitizen = citizenCreator.Create();
            var party = partyCreator.Create();

            Assert.IsFalse(entityService.CanChangeInto(ourCitizen.Entity, party.Entity, ourCitizen));
        }

        [TestMethod]
        public void ErrorOnChangeIntoPartyAsNormalMember()
        {
            var ourCitizen = citizenCreator.Create();
            partyCreator.AddMember(ourCitizen, PartyRoleEnum.Member);
            var ourParty = partyCreator.Create();

            Assert.IsFalse(entityService.CanChangeInto(ourCitizen.Entity, ourParty.Entity, ourCitizen));
        }

        [TestMethod]
        public void CanChangeIntoPartyAsManager()
        {
            var ourCitizen = citizenCreator.Create();
            partyCreator.AddMember(ourCitizen, PartyRoleEnum.Manager);
            var ourParty = partyCreator.Create();

            Assert.IsTrue(entityService.CanChangeInto(ourCitizen.Entity, ourParty.Entity, ourCitizen));
        }

        [TestMethod]
        public void CanChangeIntoPartyAsPresident()
        {
            var ourCitizen = citizenCreator.Create();
            partyCreator.AddMember(ourCitizen, PartyRoleEnum.President);
            var ourParty = partyCreator.Create();

            Assert.IsTrue(entityService.CanChangeInto(ourCitizen.Entity, ourParty.Entity, ourCitizen));
        }

        private (List<Entity> entities, List<ReservedEntityName> reserved) prepareNameTakenTest()
        {
            var creator = new CitizenDummyCreator();
            List<Entity> entities = new List<Entity>()
            {
                creator.SetName("Test").Create().Entity,
                creator.SetName("Te st").Create().Entity,
            };


            entityRepository.Setup(x => x.Any(It.IsAny<Expression<Func<Entity, bool>>>()))
                .Returns<Expression<Func<Entity, bool>>>(expr => entities.Any(expr.Compile()));
            

            List<ReservedEntityName> reserved = new List<ReservedEntityName>()
            {
                new ReservedEntityName() { ID = 1, Name = "Sum" },
                new ReservedEntityName() { ID = 2, Name = "Su m" },
            };

            reservedEntityNameRepository.Setup(x => x.Any(It.IsAny<Expression<Func<ReservedEntityName, bool>>>()))
                .Returns<Expression<Func<ReservedEntityName, bool>>>(expr => reserved.Any(expr.Compile()));
            

            return (entities, reserved);
        }

        [TestMethod]
        public void IsNameTakenNotTakenTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsFalse(entityService.IsNameTaken("rolnik"));
        }

        [TestMethod]
        public void IsNameTakenFromEntityTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("Test"));
        }

        [TestMethod]
        public void IsNameTakenFromEntityWithSpaceTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("Te st"));
        }

        [TestMethod]
        public void IsNameTakenFromEntityWithMultipleSpaceTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("Te     st"));
        }

        [TestMethod]
        public void IsNameTakenFromEntityWithMultipleSpaceAndTrimmingTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("           Te          st            "));
        }

        [TestMethod]
        public void IsNameTakenFromReservedTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("Sum"));
        }

        [TestMethod]
        public void IsNameTakenFromReservedWithSpaceTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("Su     m"));
        }

        [TestMethod]
        public void IsNameTakenFromReservedWitMultipleSpacesTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("Su            m"));
        }

        [TestMethod]
        public void IsNameTakenFromReservedWitMultipleSpacesAndTrimTest()
        {
            var x = prepareNameTakenTest();

            Assert.IsTrue(entityService.IsNameTaken("           Su            m             "));
        }


    }
}
