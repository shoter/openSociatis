using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams;
using WebServices.Helpers;
using Entities.Extensions;
using Common.Operations;
using WebServices.structs;
using Common.Extensions;

namespace WebServices
{
    public class CountryService : ICountryService
    {
        ICountryRepository countryRepository;
        IEntityRepository entityRepository;
        IEntityService entityService;
        IPresidentVotingRepository presidentVotingRepository;
        ICongressCandidateVotingRepository congressCandidateVotingRepository;
        ICitizenRepository citizenRepository;
        ICitizenService citizenService;
        ICountryPresidentService countryPresidentService;
        ICongressVotingService congressVotingService;
        IWalletService walletService;
        private readonly IWarningService warningService;


        public CountryService(ICountryRepository countriesRepository, IEntityRepository entititesRepository, IEntityService entityService,
            IPresidentVotingRepository presidentVotingRepository, ICongressCandidateVotingRepository congressCandidateVotingRepository,
            ICitizenRepository citizenRepository, ICitizenService citizenService, ICountryPresidentService countryPresidentService,
            ICongressVotingService congressVotingService, IWalletService walletService, IWarningService warningService)
        {
            this.countryRepository = countriesRepository;
            this.entityRepository = entititesRepository;
            this.entityService = entityService;
            this.presidentVotingRepository = presidentVotingRepository;
            this.congressCandidateVotingRepository = congressCandidateVotingRepository;
            this.citizenRepository = citizenRepository;
            this.citizenService = citizenService;
            this.countryPresidentService = countryPresidentService;
            this.congressVotingService = congressVotingService;
            this.walletService = walletService;
            this.warningService = warningService;
        }

        public Country CreateCountry(CreateCountryParameters parameters)
        {
            var entity = entityService.CreateEntity(parameters.CountryName, EntityTypeEnum.Country);

            Country country = new Country()
            {
                ID = entity.EntityID,
                Color = parameters.Color,
                CountryPolicy = new CountryPolicy(),
                GreetingMessage = $"Welcome in {parameters.CountryName}!"
            };
            var policy = country.CountryPolicy;
            policy.CountryCompanyBuildLawAllowHolder = (int)LawAllowHolderEnum.PresidentAndCongress;
            policy.TreasuryVisibilityLawAllowHolderID = (int)LawAllowHolderEnum.PresidentAndCongress;
            policy.CountryID = entity.EntityID;

            Currency currency = new Currency()
            {
                ShortName = parameters.CurrencyShortName,
                Name = parameters.CurrencyName,
                Symbol = parameters.CurrencySymbol,
                ID = parameters.CurrencyID
            };
            country.Currency = currency;

            countryRepository.Add(country);
            countryRepository.SaveChanges();

            CreateNewPresidentVoting(country, GameHelper.CurrentDay + country.CountryPolicy.PresidentCadenceLength);
            CreateNewCongressCandidateVoting(country, GameHelper.CurrentDay + country.CountryPolicy.CongressCadenceLength);
            Console.WriteLine($"Country {parameters.CountryName} created!");
            return country;
        }

        public MethodResult CanCreateCountryCompany(Entity entity, Country country, Citizen loggedCitizen)
        {
            if (country == null)
                return new MethodResult("Country is not defined!");

            var policy = (LawAllowHolderEnum)countryRepository.GetCountryPolicySetting(country.ID, pol => pol.CountryCompanyBuildLawAllowHolder);

            if (entity.GetEntityType() != EntityTypeEnum.Citizen)
                return new MethodResult("Only citizens can create national companies!");

            if (country.PresidentID == entity.EntityID && policy == LawAllowHolderEnum.Congress)
                return new MethodResult("Only congress can create companies in your country!");
            if (congressVotingService.IsCongressman(loggedCitizen, country) && policy == LawAllowHolderEnum.President)
                return new MethodResult("Only president can create companies in your country!");

            var neededGold = ConfigurationHelper.Configuration.CompanyCountryFee;
            var treasureGold = walletService.GetWalletMoney(country.Entity.WalletID, GameHelper.Gold.ID);

            if (treasureGold.Amount < neededGold)
                return new MethodResult($"Not enough gold to construct company. You need {neededGold} gold and your country only have {treasureGold.Amount} gold");

            return MethodResult.Success;
        }

        public MethodResult CanCreateCountryCompany(string companyName, Entity entity, Country country, Citizen loggedCitizen, Region region, ProductTypeEnum productType)
        {
            MethodResult result = CanCreateCountryCompany(entity, country, loggedCitizen);

            if (result.IsError)
                return result;

            if (Enum.IsDefined(typeof(ProductTypeEnum), productType) == false)
                return new MethodResult("Wrong product!");

            if (region.CountryID != country.ID)
                return new MethodResult("You cannot construct company in foreign region!");

            if (entityService.IsNameTaken(companyName))
                return new MethodResult("Name for company is already taken!");

            return MethodResult.Success;
        }


        public MethodResult CanCandidateAsPresident(Citizen citizen, Country country)
        {
            if (citizen.Entity.GetCurrentCountry().ID != country.ID)
                return new MethodResult("You must be at the country controlled territory to candidate!");

            if (citizen.CitizenshipID != country.ID)
                return new MethodResult("You must be citizen of this country to candidate!");


            if (countryPresidentService.IsActuallyCandidating(citizen))
                return new MethodResult("You can be candidate in one elections at same time!");

            var voting = country.PresidentVotings.Last();

            if (voting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
                return new MethodResult("President voting has started and you cannot candidate!");

            if (countryPresidentService.IsPresidentExcludingCountries(citizen, country.ID))
                return new MethodResult("You cannot candidate as president when you are president of other country!");

            return MethodResult.Success;
        }

        public MethodResult CanVoteInPresidentElections(Citizen citizen, PresidentVoting voting)
        {
            var country = voting.Country;

            if (citizen.Entity.GetCurrentCountry().ID != country.ID)
                return new MethodResult("You are not on the terrain of the country to vote.");

            if (citizen.CitizenshipID != country.ID)
                return new MethodResult("You are on an citizen of this country to be able to participate in voting.");

            if (voting.VotingStatusID != (int)VotingStatusEnum.Ongoing)
                return new MethodResult("You cannot vote");

            if (voting.PresidentVotes.Any(v => v.CitizenID == citizen.ID))
                return new MethodResult("You already voted in this voting!");

            return MethodResult.Success;
        }

        public MethodResult CanVoteOnPresidentCandidate(Citizen citizen, PresidentCandidate candidate)
        {
            MethodResult result;

            if (candidate.CandidateStatusID != (int)PresidentCandidateStatusEnum.WaitingForElectionEnd) //it will be checked without doing SQL
                return new MethodResult("You cannot vote on this candidate");


            if ((result = CanVoteInPresidentElections(citizen, candidate.PresidentVoting)).IsError)
                return result;

            return MethodResult.Success;
        }
        public void CandidateAsPresident(Citizen citizen, Country country)
        {
            var candidate = new PresidentCandidate()
            {
                CandidateID = citizen.ID,
                CandidateStatusID = (int)PresidentCandidateStatusEnum.WaitingForElectionEnd,
                VotingID = country.PresidentVotings.Last().ID
            };

            presidentVotingRepository.AddCandidate(candidate);
            presidentVotingRepository.SaveChanges();
        }

        public void VoteOnPresidentCandidate(Citizen citizen, PresidentCandidate candidate)
        {
            var vote = new PresidentVote()
            {
                CandidateID = candidate.ID,
                CitizenID = citizen.ID,
                PresidentVotingID = candidate.VotingID
            };

            presidentVotingRepository.AddVote(vote);
            presidentVotingRepository.SaveChanges();
        }

        public PresidentVoting CreateNewPresidentVoting(Entities.Country country, int votingDay)
        {
            var voting = new PresidentVoting()
            {
                CountryID = country.ID,
                StartDay = votingDay,
                VotingStatusID = (int)VotingStatusEnum.NotStarted
            };

            presidentVotingRepository.Add(voting);

            presidentVotingRepository.SaveChanges();


            return voting;
        }

        private CongressCandidateVoting CreateNewCongressCandidateVoting(Entities.Country country, int votingDay)
        {
            var voting = new CongressCandidateVoting()
            {
                CountryID = country.ID,
                VotingDay = votingDay,
                VotingStatusID = (int)VotingStatusEnum.NotStarted
            };

            congressCandidateVotingRepository.Add(voting);
            congressCandidateVotingRepository.SaveChanges();

            return voting;
        }

        public void ProcessDayChange(int newDay)
        {
            var countries = countryRepository.GetAll();
            foreach (var country in countries)
            {
                var policy = country.CountryPolicy;
                PresidentVoting presidentVoting = countryRepository.GetLastPresidentVoting(country.ID);
                ProcessPresidentVoting(newDay, country, presidentVoting);
                CorrectBadPolicy(ref policy);
            }
            CancelInactiveNotLastPresidentVotings(newDay);
            ModifyIncorrectPresidentCandidates();
            presidentVotingRepository.SaveChanges();
        }

        public void CorrectBadPolicy(ref CountryPolicy policy)
        {
            if (policy.NormalCongressVotingWinPercentage < (Constants.MinimalVotingPercentage / 100m))
                policy.NormalCongressVotingWinPercentage = (Constants.MinimalVotingPercentage / 100m);

            if (policy.NormalCongressVotingWinPercentage > (Constants.MaximalVotingPercentage / 100m))
                policy.NormalCongressVotingWinPercentage = (Constants.MaximalVotingPercentage / 100m);

            if (policy.NormalCongressVotingWinPercentage > 1m)
                policy.NormalCongressVotingWinPercentage = 1m;

            if (policy.MinimumContractLength < Constants.ContractMinimumLength)
                policy.MinimumContractLength = Constants.ContractMinimumLength;

            if (policy.MinimumContractLength > Constants.ContractMaximumLength)
                policy.MinimumContractLength = Constants.ContractMaximumLength;
            //-----------------------------------------------------------------------
            if (policy.MaximumContractLength > Constants.ContractMaximumLength)
                policy.MaximumContractLength = Constants.ContractMaximumLength;

            if (policy.MaximumContractLength < policy.MinimumContractLength)
                policy.MaximumContractLength = policy.MinimumContractLength;

        }

        //There was issue with old votings in the system that had incorrect state.We need to manually stop them.Otherwise game logic will not function properly.
        public void CancelInactiveNotLastPresidentVotings(int newDay)
        {
            var votings = presidentVotingRepository.GetInactiveNotLastPresidentVotings(newDay);
            foreach (var voting in votings)
            {
                voting.VotingStatusID = (int)VotingStatusEnum.Finished;
            }
        }

        public void ModifyIncorrectPresidentCandidates()
        {
            var candidates = presidentVotingRepository.GetCandidatesInIncorrectState();

            foreach (var candidate in candidates)
            {
                candidate.CandidateStatusID = (int)PresidentCandidateStatusEnum.Rejected;
            }
        }


        public void ProcessPresidentVoting(int newDay, Country country, PresidentVoting presidentVoting)
        {
            if (presidentVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted)
            {
                if (presidentVoting.PresidentCandidates.Count == 0)
                {
                    presidentVoting.StartDay = GameHelper.CurrentDay + 7;
                }
                else if (newDay >= presidentVoting.StartDay)
                {
                    dismissPresident(country);
                    presidentVoting.VotingStatusID = (int)VotingStatusEnum.Ongoing;

                }
            }
            else if (presidentVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
            {
                var votes = presidentVoting.PresidentVotes
                    .GroupBy(v => v.CandidateID)
                    .Select(v => new { CandidateID = v.Key, VoteCount = v.Count() })
                    .OrderByDescending(v => v.VoteCount)
                    .ToList();

                foreach (var vote in votes)
                {
                    var candidate = presidentVotingRepository.GetCandidateByID(vote.CandidateID);
                    candidate.CandidateStatusID = (int)PresidentCandidateStatusEnum.Rejected;
                }

                if (votes.Count == 0)
                {
                    presidentVoting.VotingStatusID = (int)VotingStatusEnum.Finished;
                    CreateNewPresidentVoting(country, newDay + 7);

                    var message = $"No one was elected in last president elections in {presidentVoting.Country.Entity.Name} due to lack of votes on candidates. If you still want to be a president candidate you need to candidate again";
                    foreach (var candidate in presidentVoting.PresidentCandidates)
                        warningService.AddWarning(candidate.CandidateID, message);
                }
                else if (votes.Count != 1 && votes[0].VoteCount == votes[1].VoteCount)
                {
                    //reelection in next week
                    presidentVoting.VotingStatusID = (int)VotingStatusEnum.Finished;
                    CreateNewPresidentVoting(country, newDay + 7);
                }
                else
                {
                    var policy = country.CountryPolicy;

                    var candidate = presidentVotingRepository.GetCandidateByID(votes[0].CandidateID);
                    country.PresidentID = candidate.CandidateID;
                    var gold = countryPresidentService.GetGoldForCadency(policy.PresidentCadenceLength);
                    citizenService.ReceivePresidentMedal(candidate.Citizen, gold);

                    CreateNewPresidentVoting(country, newDay + policy.PresidentCadenceLength);
                    presidentVoting.VotingStatusID = (int)VotingStatusEnum.Finished;
                    candidate.CandidateStatusID = (int)PresidentCandidateStatusEnum.Approved;
                }
            }
        }



        private void dismissPresident(Country country)
        {
            country.PresidentID = null;
        }

        public bool IsPresident(Country country, Entity entity)
        {
            return country.PresidentID == entity.EntityID || country.ID == entity.EntityID;
        }


    }
}
