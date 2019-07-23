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
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.BigParams;
using WebServices.Helpers;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class CountryServiceTests
    {

        private Mock<ICountryRepository> countryRepository = new Mock<ICountryRepository>();
        private Mock<IEntityRepository> entityRepository = new Mock<IEntityRepository>();
        private Mock<IPresidentVotingRepository> presidentVotingRepository = new Mock<IPresidentVotingRepository>();
        private Mock<ICongressCandidateVotingRepository> congressCandidateVotingRepository = new Mock<ICongressCandidateVotingRepository>();
        private Mock<ICitizenRepository> citizenRepository = new Mock<ICitizenRepository>();
        List<PresidentCandidate> candidates = new List<PresidentCandidate>();

        private IEntityService entityService;
        private CountryService countrySerivce => mockCountryService.Object;
        private Mock<CountryService> mockCountryService;

        

        public CountryServiceTests()
        {
            entityService = new EntityService(entityRepository.Object, Mock.Of<IReservedEntityNameRepository>());

            mockCountryService = new Mock<CountryService>(countryRepository.Object, entityRepository.Object, entityService,
                presidentVotingRepository.Object, congressCandidateVotingRepository.Object, citizenRepository.Object,
                Mock.Of<ICitizenService>(), Mock.Of<ICountryPresidentService>(), Mock.Of<ICongressVotingService>(), Mock.Of<IWalletService>());

            presidentVotingRepository.Setup(x => x.GetCandidateByID(It.IsAny<int>()))
            .Returns<int>(id => candidates.First(c => c.ID == id));

            SingletonInit.Init();

        }

        [TestMethod]
        public void CreateNewCountryTest()
        {

            var param = new CreateCountryParameters()
            {
                CountryName = "Test Country",
                CurrencyName = "Test",
                CurrencyShortName = "TST",
                CurrencySymbol = "T"
            };

            Country createdCountry = null;
            PresidentVoting createdVoting = null;
            CongressCandidateVoting congressVoting = null;

            presidentVotingRepository.Setup(x => x.Add(It.IsAny<PresidentVoting>()))
                .Callback<PresidentVoting>(v => createdVoting = v);

            countryRepository.Setup(x => x.Add(It.IsAny<Country>()))
                .Callback<Country>(c => {
                    createdCountry = c;
                    c.CountryPolicy.CongressCadenceLength = 7;
                    c.CountryPolicy.PresidentCadenceLength = 7;
                });

            congressCandidateVotingRepository.Setup(x => x.Add(It.IsAny<CongressCandidateVoting>()))
                .Callback<CongressCandidateVoting>(v => congressVoting = v);

            var country = countrySerivce.CreateCountry(param);



            entityRepository.Verify(x => x.Add(It.Is<Entity>(e => e.Name == param.CountryName)), Times.Once);
            Assert.AreEqual(param.CurrencyName, createdCountry.Currency.Name);
            Assert.AreEqual(param.CurrencyShortName, createdCountry.Currency.ShortName);
            Assert.AreEqual(param.CurrencySymbol, createdCountry.Currency.Symbol);

            Assert.IsTrue(createdVoting.StartDay > GameHelper.CurrentDay);
            Assert.IsTrue(congressVoting.VotingDay > GameHelper.CurrentDay);
        }

        [TestMethod]
        public void PresidentVotingTestNoCandidates()
        {
            var countryCreator = new CountryDummyCreator();

            countryCreator.VotingCreator.SetState(GameHelper.CurrentDay, VotingStatusEnum.NotStarted);

            var country = countryCreator.Create();

            var voting = country.PresidentVotings.Last();

            countrySerivce.ProcessPresidentVoting(GameHelper.CurrentDay, country, voting);

            Assert.IsTrue(country.PresidentID == null);
            Assert.IsTrue(voting.VotingStatusID == (int)VotingStatusEnum.NotStarted);
            Assert.IsTrue(voting.StartDay > GameHelper.CurrentDay);
        }

        [TestMethod]
        public void PresidentVotingTestNoVotes()
        {
            var countryCreator = new CountryDummyCreator();
            var candidatesCreator = new PresidentCandidateDummyCreator();
            countryCreator.VotingCreator.SetState(GameHelper.CurrentDay, VotingStatusEnum.Ongoing);

            var country = countryCreator.Create();

            var citizenCreator = new CitizenDummyCreator();

            var voting = country.PresidentVotings.Last();

            for (int i = 0; i < 10; ++i)
                voting.PresidentCandidates.Add(candidatesCreator.Create(voting));

            PresidentVoting newVoting = null;

            presidentVotingRepository.Setup(x => x.Add(It.IsAny<PresidentVoting>()))
               .Callback<PresidentVoting>(v => newVoting = v);

            countrySerivce.ProcessPresidentVoting(GameHelper.CurrentDay, country, voting);

            Assert.IsTrue(country.PresidentID == null);
            Assert.IsTrue(voting.VotingStatusID == (int)VotingStatusEnum.Finished);
            Assert.IsTrue(voting != newVoting && newVoting != null);
            Assert.IsTrue(newVoting.StartDay > GameHelper.CurrentDay);
            Assert.IsTrue(newVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted);
        }

        [TestMethod]
        public void PresidentVotingSameNumberOfVotes()
        {
            var countryCreator = new CountryDummyCreator();
            var candidatesCreator = new PresidentCandidateDummyCreator();
            candidatesCreator.setVotesNumber(10);

            countryCreator.VotingCreator.SetState(GameHelper.CurrentDay, VotingStatusEnum.Ongoing);

            var country = countryCreator.Create();

            var citizenCreator = new CitizenDummyCreator();

            var voting = country.PresidentVotings.Last();

            for (int i = 0; i < 5; ++i)
            {
                var candidate = candidatesCreator.Create(voting);
                voting.PresidentCandidates.Add(candidate);
                candidates.Add(candidate);
            }

            PresidentVoting newVoting = null;

            presidentVotingRepository.Setup(x => x.Add(It.IsAny<PresidentVoting>()))
               .Callback<PresidentVoting>(v => newVoting = v);

            countrySerivce.ProcessPresidentVoting(GameHelper.CurrentDay, country, voting);

            Assert.IsTrue(country.PresidentID == null);
            Assert.IsTrue(voting.VotingStatusID == (int)VotingStatusEnum.Finished);
            Assert.IsTrue(voting != newVoting && newVoting != null);
            Assert.IsTrue(newVoting.StartDay > GameHelper.CurrentDay);
            Assert.IsTrue(newVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted);
        }

        [TestMethod]
        public void PresidentVotingCandidateWinningTest()
        {
            
            var countryCreator = new CountryDummyCreator();
            var candidatesCreator = new PresidentCandidateDummyCreator();
           

            countryCreator.VotingCreator.SetState(GameHelper.CurrentDay, VotingStatusEnum.Ongoing);

            var country = countryCreator.Create();

            var citizenCreator = new CitizenDummyCreator();

            var voting = country.PresidentVotings.Last();

            PresidentCandidate lastCandidate = null;

            


            for (int i = 0; i < 5; ++i)
            {
                candidatesCreator.setVotesNumber(10 + i);
                var candidate = candidatesCreator.Create(voting);
                voting.PresidentCandidates.Add(candidate);
                lastCandidate = candidate;
                candidates.Add(candidate);
            }

            PresidentVoting newVoting = null;

            presidentVotingRepository.Setup(x => x.Add(It.IsAny<PresidentVoting>()))
               .Callback<PresidentVoting>(v => newVoting = v);

          

            countrySerivce.ProcessPresidentVoting(GameHelper.CurrentDay, country, voting);

            Assert.IsTrue(country.PresidentID == lastCandidate.CandidateID);
            Assert.IsTrue(lastCandidate.CandidateStatusID == (int)PresidentCandidateStatusEnum.Approved);
            Assert.IsTrue(voting.VotingStatusID == (int)VotingStatusEnum.Finished);
            Assert.IsTrue(voting != newVoting && newVoting != null);
            Assert.IsTrue(newVoting.StartDay > GameHelper.CurrentDay);
            Assert.IsTrue(newVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted);
            foreach (var candidate in candidates)
            {
                if(candidate != lastCandidate)
                    Assert.IsTrue(candidate.CandidateStatusID == (int)PresidentCandidateStatusEnum.Rejected);
            }
        }

        [TestMethod]
        public void CandidateAsPresidentTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();

            var citizen = new CitizenDummyCreator().Create();

            PresidentCandidate newCandidate = null;

            presidentVotingRepository.Setup(x => x.AddCandidate(It.IsAny<PresidentCandidate>()))
                .Callback<PresidentCandidate>(c => newCandidate = c);

            countrySerivce.CandidateAsPresident(citizen, country);

            Assert.IsTrue(newCandidate.CandidateID == citizen.ID);
            Assert.IsTrue(newCandidate.VotingID == country.PresidentVotings.Last().ID);


        }

        [TestMethod]
        public void CanCandidateWrongCountryTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();
            var otherCountry = countryCreator.Create();

            var citizen = new CitizenDummyCreator().SetCountry(otherCountry).Create();

            Assert.IsFalse(countrySerivce.CanCandidateAsPresident(citizen, country).isSuccess);
        }

        [TestMethod]
        public void CanCandidateGoodCountryTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();

            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            Assert.IsTrue(countrySerivce.CanCandidateAsPresident(citizen, country).isSuccess);
        }


        [TestMethod]
        public void CanCandidateOngoingTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();


            var citizen = new CitizenDummyCreator().SetCountry(country).Create();

            country.PresidentVotings.Last().VotingStatusID = (int)VotingStatusEnum.Ongoing;
            country.PresidentVotings.Last().StartDay = GameHelper.CurrentDay;

            Assert.IsFalse(countrySerivce.CanCandidateAsPresident(citizen, country).isSuccess);
        }

        [TestMethod]
        public void CanCandidateWrongCitizenshipTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();
            var otherCountry = countryCreator.Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(otherCountry)
                .SetRegion(country.Regions.First())
                .Create();

            Assert.IsFalse(countrySerivce.CanCandidateAsPresident(citizen, country).isSuccess); 
        }

        [TestMethod] 
        public void CanVoteInPresidentElectionsNotStartedTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            Assert.IsFalse(countrySerivce.CanVoteInPresidentElections(citizen, country.PresidentVotings.Last()).isSuccess);
        }

        [TestMethod]
        public void CanVoteInPresidentElectionsWrongCountryTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();
            var otherCountry = countryCreator.Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .SetRegion(otherCountry.Regions.First())
                .Create();

            Assert.IsFalse(countrySerivce.CanVoteInPresidentElections(citizen, country.PresidentVotings.Last()).isSuccess);
        }

        [TestMethod]
        public void CanVoteInPresidentElectionsWrongCitizenshipTest()
        {
            var countryCreator = new CountryDummyCreator();

            var country = countryCreator.Create();
            var otherCountry = countryCreator.Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(otherCountry)
                .SetRegion(country.Regions.First())
                .Create();

            Assert.IsFalse(countrySerivce.CanVoteInPresidentElections(citizen, country.PresidentVotings.Last()).isSuccess);
        }

        [TestMethod]
        public void CanVoteInPresidentElectionsNotOngoingTest()
        {
            var country = new CountryDummyCreator()
                .SetPresidentVotingStatus(GameHelper.CurrentDay - 1, VotingStatusEnum.NotStarted)
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            Assert.IsFalse(countrySerivce.CanVoteInPresidentElections(citizen, country.PresidentVotings.Last()).isSuccess);
        }

        [TestMethod]
        public void CanVoteInPresidentElectionsVotedBeforeTest()
        {
            var country = new CountryDummyCreator()
                .SetPresidentVotingStatus(GameHelper.CurrentDay, VotingStatusEnum.Ongoing)
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var candidateCitizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var voting = country.PresidentVotings.Last();

            var candidate = new PresidentCandidateDummyCreator()
                .Create(voting);

            var vote = new PresidentVoteDummyGenerator()
                .SetVotingCitizen(citizen)
                .Create(candidate);

            Assert.IsFalse(countrySerivce.CanVoteInPresidentElections(citizen, country.PresidentVotings.Last()).isSuccess);
        }

        [TestMethod]
        public void CanVoteInPresidentElectionsTest()
        {
            var country = new CountryDummyCreator()
                .SetPresidentVotingStatus(GameHelper.CurrentDay, VotingStatusEnum.Ongoing)
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var candidateCitizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var voting = country.PresidentVotings.Last();

            var candidate = new PresidentCandidateDummyCreator()
                .Create(voting);

            Assert.IsTrue(countrySerivce.CanVoteInPresidentElections(citizen, country.PresidentVotings.Last()).isSuccess);
            Assert.IsTrue(countrySerivce.CanVoteInPresidentElections(candidateCitizen, country.PresidentVotings.Last()).isSuccess);
        }

        [TestMethod]
        public void CanVoteOnPresidentCandidatePreviousVoting()
        {
            var country = new CountryDummyCreator()
                .SetPresidentVotingStatus(GameHelper.CurrentDay - 7, VotingStatusEnum.Finished)
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var candidateCitizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var oldVoting = country.PresidentVotings.Last();

            var newVoting = new PresidentVotingDummyCreator()
                .SetState(GameHelper.CurrentDay, VotingStatusEnum.Ongoing)
                .Create(country);
                

            var oldCandidate = new PresidentCandidateDummyCreator()
                .Create(oldVoting);

            Assert.IsFalse(countrySerivce.CanVoteOnPresidentCandidate(citizen, oldCandidate).isSuccess);
        }

        [TestMethod]
        public void VoteOnPresidentCandidateTest()
        {
            bool saveChangesCalled = false;
            PresidentVote addedVote = null;

            presidentVotingRepository.Setup(x => x.AddVote(It.IsAny<PresidentVote>()))
                .Callback<PresidentVote>(v => addedVote = v);

            presidentVotingRepository.Setup(x => x.SaveChanges())
                .Callback(() => saveChangesCalled = true);

            var country = new CountryDummyCreator()
                .SetPresidentVotingStatus(GameHelper.CurrentDay, VotingStatusEnum.Ongoing)
                .Create();

            var citizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var candidateCitizen = new CitizenDummyCreator()
                .SetCountry(country)
                .Create();

            var voting = country.PresidentVotings.Last();

            var candidate = new PresidentCandidateDummyCreator()
                .Create(voting);

            countrySerivce.VoteOnPresidentCandidate(citizen, candidate);

            Assert.IsTrue(saveChangesCalled);
            Assert.AreEqual(citizen.ID, addedVote.CitizenID);
            Assert.AreEqual(candidate.ID, addedVote.CandidateID);
            Assert.AreEqual(voting.ID, addedVote.PresidentVotingID);


        }

        [TestMethod]
        public void CorrectBadPolicy_SmallMinimalVotingPercentage_Test()
        {
            var policy = new CountryPolicy() { NormalCongressVotingWinPercentage = 0.01m };

            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.MinimalVotingPercentage / 100m, policy.NormalCongressVotingWinPercentage);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMinimalVotingPercentage_Test()
        {
            decimal startPercentage = 0.6m;
            var policy = new CountryPolicy() { NormalCongressVotingWinPercentage = startPercentage };

            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(startPercentage, policy.NormalCongressVotingWinPercentage);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectOutOfBoundsVotingPercentage_Test()
        {
            decimal startPercentage = 1.1m;
            var policy = new CountryPolicy() { NormalCongressVotingWinPercentage = startPercentage };

            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.MaximalVotingPercentage / 100m, policy.NormalCongressVotingWinPercentage);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMinimalContractLength_Test()
        {
            var policy = new CountryPolicy() { MinimumContractLength = 0 };
            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.ContractMinimumLength, policy.MinimumContractLength);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMaximumContractLength_Test()
        {
            var policy = new CountryPolicy() { MaximumContractLength = 0 };
            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.ContractMinimumLength, policy.MaximumContractLength);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMinimalContractLengthTooLong_Test()
        {
            var policy = new CountryPolicy() { MinimumContractLength = Constants.ContractMaximumLength + 50 };
            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.ContractMaximumLength, policy.MinimumContractLength);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMaximumContractLengthTooLong_Test()
        {
            var policy = new CountryPolicy() { MaximumContractLength = Constants.ContractMaximumLength + 50 };
            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.ContractMaximumLength, policy.MaximumContractLength);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMaximumAccordingToMinimumTest_Test()
        {
            var policy = new CountryPolicy() { MinimumContractLength = 30, MaximumContractLength = 20 };
            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(30, policy.MaximumContractLength);
        }

        [TestMethod]
        public void CorrectBadPolicy_CorrectMaximumAccordingToMinimumTestBoth_Test()
        {
            var policy = new CountryPolicy() { MinimumContractLength = 66, MaximumContractLength = 20 };
            countrySerivce.CorrectBadPolicy(ref policy);

            Assert.AreEqual(Constants.ContractMaximumLength, policy.MaximumContractLength);
        }

    }
}
