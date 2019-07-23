using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class PartyServiceTests
    {
        Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        Mock<IPartyRepository> partyRepository = new Mock<IPartyRepository>();
        Mock<ICitizenRepository> citizenRepository = new Mock<ICitizenRepository>();
        Mock<ICongressCandidateVotingRepository> congressCandidateVotingRepository = new Mock<ICongressCandidateVotingRepository>();
        IPartyService partyService;
        IEntityService entityService;

        CitizenDummyCreator citizenCreator = new CitizenDummyCreator();
        PartyDummyCreator partyCreator = new PartyDummyCreator();
        public PartyServiceTests()
        {
            entityService = new EntityService(entityRepository.Object, Mock.Of<IReservedEntityNameRepository>());

            partyService = new PartyService(partyRepository.Object, entityService, citizenRepository.Object, congressCandidateVotingRepository.Object, Mock.Of<IPartyInviteRepository>(), Mock.Of<IPartyJoinRequestRepository>(), Mock.Of<IWarningService>(), Mock.Of<IPopupService>());
        }

        [TestMethod]
        public void CanAcceptCongressCandidatesAsPresident()
        {
            var president = citizenCreator.Create();
            partyCreator.AddMember(president, PartyRoleEnum.President);
            var party = partyCreator.Create();

            Assert.IsTrue(partyService.CanAcceptCongressCandidates(president, party));
        }

        [TestMethod]
        public void ErrorAcceptCongressCandidatesAsManager()
        {
            var president = citizenCreator.Create();
            partyCreator.AddMember(president, PartyRoleEnum.Manager);
            var party = partyCreator.Create();

            Assert.IsFalse(partyService.CanAcceptCongressCandidates(president, party));
        }

        [TestMethod]
        public void ErrorAcceptCongressCandidatesAsNormalMember()
        {
            var president = citizenCreator.Create();
            partyCreator.AddMember(president, PartyRoleEnum.Member);
            var party = partyCreator.Create();

            Assert.IsFalse(partyService.CanAcceptCongressCandidates(president, party));
        }
    }
}
