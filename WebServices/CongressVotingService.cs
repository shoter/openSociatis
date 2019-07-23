using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using WebServices.structs;
using Entities.Repository;
using WebServices.structs.Votings;
using WebServices.Helpers;
using Entities.enums;
using Entities.Extensions;
using Common.Extensions;
using Common;
using System.Transactions;
using Common.Operations;
using Common.Language;

namespace WebServices
{
    public class CongressVotingService : BaseService, ICongressVotingService
    {
        private readonly ICongressVotingRepository congressVotingRepository;
        private readonly IPartyMemberRepository partyMemberRepository;
        private readonly ICongressmenRepository congressmenRepository;
        private readonly IProductTaxRepository productTaxRepository;
        private readonly IWarningService warningService;
        private readonly ICompanyService companyService;
        private readonly IWalletService walletService;
        private readonly IRegionRepository regionRepository;
        private readonly IReservedEntityNameRepository reservedEntityNameRepository;
        private readonly ICongressVotingReservedMoneyRepository congressVotingReservedMoneyRepository;
        private readonly ITransactionsService transactionsService;
        private readonly ICompanyRepository companyRepository;
        private readonly ICitizenRepository citizenRepository;
        private readonly IVotingGreetingMessageRepository votingGreetingMessageRepository;
        private readonly IHospitalService hospitalService;
        private readonly ICountryRepository countryRepository;
        private readonly IRemovalService removalService;
        private readonly IDefenseSystemService defenseSystemService;
        private readonly IConstructionService constructionService;
        private readonly ICountryEventService countryEventService;

        public CongressVotingService(ICongressVotingRepository congressVotingRepository, IPartyMemberRepository partyMemberRepository, ICongressmenRepository congressmenRepository,
            IProductTaxRepository productTaxRepository, IWarningService warningService, ICompanyService companyService, IWalletService walletService, IRegionRepository regionRepository,
            IReservedEntityNameRepository reservedEntityNameRepository, ICongressVotingReservedMoneyRepository congressVotingReservedMoneyRepository, ITransactionsService transactionsService,
            ICompanyRepository companyRepository, ICitizenRepository citizenRepository, IVotingGreetingMessageRepository votingGreetingMessageRepository,
            IHospitalService hospitalService, ICountryRepository countryRepository, IRemovalService removalService,
            IDefenseSystemService defenseSystemService, IConstructionService constructionService, ICountryEventService countryEventService)

        {
            this.congressVotingRepository = congressVotingRepository;
            this.partyMemberRepository = partyMemberRepository;
            this.congressmenRepository = congressmenRepository;
            this.productTaxRepository = productTaxRepository;
            this.warningService = warningService;
            this.companyService = companyService;
            this.walletService = walletService;
            this.regionRepository = regionRepository;
            this.reservedEntityNameRepository = reservedEntityNameRepository;
            this.congressVotingReservedMoneyRepository = congressVotingReservedMoneyRepository;
            this.transactionsService = transactionsService;
            this.companyRepository = companyRepository;
            this.citizenRepository = citizenRepository;
            this.votingGreetingMessageRepository = votingGreetingMessageRepository;
            this.hospitalService = hospitalService;
            this.countryRepository = countryRepository;
            this.removalService = removalService;
            this.constructionService = constructionService;
            this.defenseSystemService = defenseSystemService;
            this.countryEventService = countryEventService;
        }
        public CongressVoting StartVote(StartCongressVotingParameters parameters)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                CongressVoting vote = new CongressVoting()
                {
                    CreatorID = parameters.Creator.ID,
                    CommentRestrictionID = (int)parameters.CommentRestriction,
                    CountryID = parameters.Country.ID,
                    StartTime = DateTime.Now,
                    StartDay = GameHelper.CurrentDay,
                    VotingStatusID = (int)VotingStatusEnum.Ongoing,
                    VotingTypeID = (int)parameters.VotingType,
                    CreatorMessage = parameters.CreatorMessage,
                    VotingLength = parameters.VotingLength
                };

                parameters.FillCongressVotingArguments(vote);

                var congressmen = congressmenRepository.First(c => c.CitizenID == vote.CreatorID);

                congressmen.LastVotingDay = GameHelper.CurrentDay;




                congressVotingRepository.Add(vote);
                specialVotePostProcess(vote);
                congressVotingRepository.SaveChanges();
                informAboutNewVoting(vote);
                countryEventService.AddVotingEvent(vote, VotingStatusEnum.Started);
                trs?.Complete();
                return vote;
            }
        }

        private void informAboutNewVoting(CongressVoting voting)
        {
            var link = CongressVotingLinkCreator.Create(voting).ToHtmlString();
            var msg = $"{link} has started.";
            var country = countryRepository.GetById(voting.CountryID);
            warningService.SendWarningToCongress(country, msg);
        }

        private void specialVotePostProcess(CongressVoting voting)
        {
            switch (voting.GetVotingType())
            {
                case VotingTypeEnum.CreateNationalCompany:
                    reservedEntityNameRepository.Add(voting.Argument1);
                    ReserveMoneyForVoting(voting, GetMoneyForCreatingCountryCompany());
                    break;
                case VotingTypeEnum.TransferCashToCompany:
                    ReserveMoneyForVoting(voting, getMoneyForTrasnferingCashToCompany(voting));
                    break;
                case VotingTypeEnum.PrintMoney:
                    var money = getMoneyForPrintingMoney(voting);
                    ReserveMoneyForVoting(voting, money);
                    double amount = (double)money.Amount;
                    voting.Argument2 = amount.ToString();
                    break;
                case VotingTypeEnum.RemoveNationalCompany:
                    var companyID = int.Parse(voting.Argument1);
                    var name = companyRepository.Where(c => c.ID == companyID).Select(c => c.Entity.Name).First();
                    voting.Argument2 = name;
                    break;
                case VotingTypeEnum.BuildDefenseSystem:
                    ReserveMoneyForVoting(voting, getMoneyForCreatingDefenseSystem(int.Parse(voting.Argument3)));
                    break;
                default:
                    return;
            }
        }

        private Money getMoneyForCreatingDefenseSystem(int quality)
        {
            return new Money()
            {
                Amount = defenseSystemService.GetGoldCostForStartingConstruction(quality),
                Currency = GameHelper.Gold
            };
        }

        public Money GetMoneyForCreatingCountryCompany()
        {
            return new Money()
            {
                Amount = ConfigurationHelper.Configuration.CompanyCountryFee,
                Currency = GameHelper.Gold
            };
        }

        private Money getMoneyForTrasnferingCashToCompany(CongressVoting voting)
        {
            decimal amount = decimal.Parse(voting.Argument1);
            int currencyID = int.Parse(voting.Argument3);

            return new Money(currencyID, amount);
        }

        private Money getMoneyForPrintingMoney(CongressVoting voting)
        {
            int moneyAmount = int.Parse(voting.Argument1);
            return new Money(GameHelper.Gold, (decimal)GetPrintCost(moneyAmount));
        }

        public double GetPrintCost(int moneyAmount)
        {
            return Math.Round(moneyAmount * 0.001, 2, MidpointRounding.AwayFromZero);
        }




        public CongressVotingComment AddComment(CongressVoting voting, Citizen citizen, string message)
        {
            CongressVotingComment comment = new CongressVotingComment()
            {
                Citizen = citizen,
                CongressVoting = voting,
                Day = GameHelper.CurrentDay,
                Time = DateTime.Now,
                Message = message
            };

            congressVotingRepository.AddComment(comment);
            congressVotingRepository.SaveChanges();

            return comment;
        }

        public CongressVote AddVote(CongressVoting voting, Citizen citizen, VoteTypeEnum voteType)
        {
            CongressVote vote = createVote(voting, citizen.ID, voteType);

            congressVotingRepository.AddVote(vote);
            congressVotingRepository.SaveChanges();

            return vote;
        }

        private CongressVote createVote(CongressVoting voting, int citizenID, VoteTypeEnum voteType)
        {
            return new CongressVote()
            {
                VoteTypeID = (int)voteType,
                CongressVoting = voting,
                CitizenID = citizenID
            };
        }

        public void AddAbstainedVotesForNotVoters(CongressVoting voting)
        {
            List<int> congressManVotedIDs = voting.CongressVotes
                .Select(v => v.Citizen.ID)
                .ToList();



            var abstainedCongressmans = voting.Country
                .Congressmen
                .Where(c => congressManVotedIDs.Contains(c.CitizenID) == false)
                .ToList();

            foreach (var abstained in abstainedCongressmans)
            {
                var vote = createVote(voting, abstained.CitizenID, VoteTypeEnum.Abstained);

                congressVotingRepository.AddVote(vote);
            }

            congressVotingRepository.SaveChanges();
        }

        public bool IsCongressman(Citizen citizen, Country country)
        {
            return country.Congressmen
                .Any(c => c.CitizenID == citizen.ID);
        }

        public bool HasVoted(Citizen citizen, CongressVoting voting)
        {
            return voting.
                CongressVotes
                .Any(v => v.CitizenID == citizen.ID);
        }

        public bool CanVote(Citizen citizen, CongressVoting voting)
        {
            var isCongressman = voting.Country.Congressmen.Any(c => c.CitizenID == citizen.ID);


            return
                voting.CongressVotes.Any(c => c.CitizenID == citizen.ID) == false && isCongressman && voting.VotingStatusID == (int)VotingStatusEnum.Ongoing;
        }

        public void ProcessDayChange(int newDay)
        {
            var unfinishedVotings = congressVotingRepository.Where(v => v.VotingStatusID == (int)VotingStatusEnum.Ongoing).ToList();
            foreach (var voting in unfinishedVotings)
            {
                var country = voting.Country;

                var timeLeft = voting.GetTimeLeft(GameHelper.CurrentDay);

                if (timeLeft.TotalSeconds < 0)
                {
                    var votes = voting.CongressVotes.ToList();
                    var supportVotes = votes.Where(v => v.VoteTypeID == (int)VoteTypeEnum.Yes);
                    var againstVotes = votes.Where(v => v.VoteTypeID == (int)VoteTypeEnum.No);

                    var supportersIDs = supportVotes.Select(s => s.CitizenID);
                    var againstsIDs = againstVotes.Select(a => a.CitizenID);

                    var abstained = country.Congressmen.Where(c => supportersIDs.Contains(c.CitizenID) == false && againstsIDs.Contains(c.CitizenID) == false);
                    var winPercentage = (double)country.CountryPolicy.NormalCongressVotingWinPercentage;

                    foreach (var vote in CreateAbstainedVotes(voting, abstained))
                        congressVotingRepository.AddVote(vote);

                    double percentage = (double)supportVotes.Count() / (double)(Math.Max(votes.Count(), 1));

                    if (percentage >= winPercentage)
                        using (NoSaveChanges)
                        {
                            FinishVoting(voting);
                            
                        }
                    else
                    {
                        RejectVoting(voting, CongressVotingRejectionReasonEnum.NotEnoughVotes);
                        
                    }

                    using (NoSaveChanges)
                    {
                        var votingLink = CongressVotingLinkCreator.Create(voting);
                        string message = $"{votingLink} has ended with result: {voting.GetStatus()}.";

                        if (voting.GetStatusEnum() == VotingStatusEnum.Rejected)
                            message += $"{voting.GetRejectionReason().ToHumanReadable().FirstUpper()}.";

                        warningService.SendWarningToCongress(voting.Country, message);
                    }
                }
            }

            congressVotingRepository.SaveChanges();
        }

        public void RejectVoting(CongressVoting voting, CongressVotingRejectionReasonEnum reason)
        {
            voting.VotingStatusID = (int)VotingStatusEnum.Rejected;
            voting.RejectionReasonID = (int)reason;

            switch (voting.GetVotingType())
            {
                case VotingTypeEnum.CreateNationalCompany:
                    UnreserveMoneyForVoting(voting);
                    reservedEntityNameRepository.Remove(voting.Argument1);
                    break;
                case VotingTypeEnum.TransferCashToCompany:
                    UnreserveMoneyForVoting(voting);
                    break;

                default:
                    break;
            }

            countryEventService.AddVotingEvent(voting, VotingStatusEnum.Rejected);
        }

        public void FinishVoting(CongressVoting voting)
        {
            voting.VotingStatusID = (int)VotingStatusEnum.Accepted;
            switch ((VotingTypeEnum)voting.VotingTypeID)
            {
                case VotingTypeEnum.ChangeCongressVotingLength:
                    {
                        voting.Country.CountryPolicy.CongressVotingLength = int.Parse(voting.Argument1);
                        break;
                    }
                case VotingTypeEnum.ChangePartyPresidentCadenceLength:
                    {
                        voting.Country.CountryPolicy.PartyPresidentCadenceLength = int.Parse(voting.Argument1);
                        break;
                    }
                case VotingTypeEnum.ChangeCongressCadenceLength:
                    {
                        voting.Country.CountryPolicy.CongressCadenceLength = int.Parse(voting.Argument1);
                        break;
                    }
                case VotingTypeEnum.ChangeNormalJobMarketFee:
                    {
                        voting.Country.CountryPolicy.NormalJobMarketFee = (decimal)(double.Parse(voting.Argument1));
                        break;
                    }
                case VotingTypeEnum.ChangeContractJobMarketFee:
                    {
                        voting.Country.CountryPolicy.ContractJobMarketFee = (decimal)(double.Parse(voting.Argument1));
                        break;
                    }
                case VotingTypeEnum.ChangeProductVAT:
                    {
                        ProductTypeEnum productType = ((ProductTypeEnum)int.Parse(voting.Argument1));
                        var rate = double.Parse(voting.Argument2);

                        var tax = productTaxRepository.FirstOrDefault(t => t.ProductID == (int)productType &&
                        t.CountryID == voting.CountryID);

                        if (tax == null)
                        {
                            tax = new ProductTax()
                            {
                                CountryID = voting.CountryID,
                                ProductID = (int)productType,
                                ProductTaxTypeID = (int)ProductTaxTypeEnum.VAT,
                                TaxRate = (decimal)(rate / 100.0),
                                ForeignCountryID = voting.CountryID
                            };
                            productTaxRepository.Add(tax);
                        }
                        else
                            tax.TaxRate = (decimal)rate;

                        productTaxRepository.SaveChanges();

                        break;
                    }
                case VotingTypeEnum.ChangeProductExportTax:
                case VotingTypeEnum.ChangeProductImportTax:
                    {
                        ProductTypeEnum productType = ((ProductTypeEnum)int.Parse(voting.Argument1));
                        var rate = double.Parse(voting.Argument2);
                        int countryID = voting.CountryID;
                        int foreignCountryID = int.Parse(voting.Argument3);

                        if (foreignCountryID != -1)
                            addForeignTax(voting, productType, rate, foreignCountryID);
                        else
                        {
                            foreach (var country in Persistent.Countries.GetAll())
                            {
                                if (country.ID == countryID)
                                    continue;
                                addForeignTax(voting, productType, rate, country.ID);
                            }
                        }

                        productTaxRepository.SaveChanges();

                        break;
                    }
                case VotingTypeEnum.ChangeArticleTax:
                    {
                        var taxRate = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.ArticleTax = (decimal)(taxRate / 100.0);
                        break;
                    }
                case VotingTypeEnum.ChangeNewspaperCreateCost:
                    {
                        var cost = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.NewspaperCreateCost = (decimal)cost;
                        break;
                    }
                case VotingTypeEnum.ChangeMarketOfferCost:
                    {
                        var cost = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.MarketOfferCost = (decimal)cost;
                        break;
                    }
                case VotingTypeEnum.ChangeOrganisationCreateCost:
                    {
                        var cost = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.OrganisationCreateCost = (decimal)cost;
                        break;
                    }
                case VotingTypeEnum.ChangePresidentCadenceLength:
                    {
                        var length = int.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.PresidentCadenceLength = length;
                        break;
                    }
                case VotingTypeEnum.ChangePartyCreateFee:
                    {
                        var cost = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.PartyFoundingFee = (decimal)cost;
                        break;
                    }
                case VotingTypeEnum.ChangeNormalCongressVotingWinPercentage:
                    {
                        var winPercentage = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.NormalCongressVotingWinPercentage = (decimal)(winPercentage / 100.0);
                        break;
                    }
                case VotingTypeEnum.ChangeCitizenCompanyCost:
                case VotingTypeEnum.ChangeOrganisationCompanyCost:
                    {
                        var cost = double.Parse(voting.Argument1);

                        if ((VotingTypeEnum)voting.VotingTypeID == VotingTypeEnum.ChangeCitizenCompanyCost)
                            voting.Country.CountryPolicy.CitizenCompanyCost = (decimal)cost;
                        else
                            voting.Country.CountryPolicy.OrganisationCompanyCost = (decimal)cost;
                        break;
                    }
                case VotingTypeEnum.ChangeMonetaryTaxRate:
                    {
                        var rate = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.MonetaryTaxRate = (decimal)rate / 100;
                        break;
                    }
                case VotingTypeEnum.ChangeMinimumMonetaryTaxValue:
                    {
                        var value = double.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.MinimumMonetaryTax = (decimal)value;
                        break;
                    }
                case VotingTypeEnum.ChangeTreasureLawHolder:
                    {
                        var value = int.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.TreasuryVisibilityLawAllowHolderID = value;
                        break;
                    }
                case VotingTypeEnum.ChangeCompanyCreationLawHolder:
                    {
                        var value = int.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.CountryCompanyBuildLawAllowHolder = value;
                        break;
                    }
                case VotingTypeEnum.CreateNationalCompany:
                    {
                        var country = voting.Country;
                        var regionID = Convert.ToInt32(voting.Argument2);

                        var region = regionRepository.GetById(regionID);

                        if (region.CountryID != voting.CountryID)
                        {
                            RejectVoting(voting, CongressVotingRejectionReasonEnum.RegionIsNotYoursConstructCompany);
                            break;
                        }

                        var companyName = voting.Argument1;
                        ProductTypeEnum productType = (ProductTypeEnum)Convert.ToInt32(voting.Argument3);

                        if (productType == ProductTypeEnum.MedicalSupplies)
                        {
                            companyName = hospitalService.GenerateNameForHospital(region);
                        }

                        reservedEntityNameRepository.Remove(companyName);
                        var company = companyService.CreateCompanyForCountry(companyName, productType, regionID, country, payForCreation: false);

                        if (productType == ProductTypeEnum.MedicalSupplies)
                            region.Hospital = company.Hospital;

                        RemoveFromGameMoneyInVoting(voting);
                        break;
                    }
                case VotingTypeEnum.RemoveNationalCompany:
                    {
                        int companyID = int.Parse(voting.Argument1);
                        var company = companyRepository.GetById(companyID);
                        if (company.Region.CountryID != voting.CountryID)
                        {
                            RejectVoting(voting, CongressVotingRejectionReasonEnum.CompanyIsNotYoursRemoveCompany);
                            break;
                        }
                        removalService.RemoveCompany(company);
                        break;
                    }
                case VotingTypeEnum.AssignManagerToCompany:
                    {
                        var companyID = int.Parse(voting.Argument1);
                        var citizenID = int.Parse(voting.Argument2);

                        var company = companyRepository.GetById(companyID);
                        var citizen = citizenRepository.GetById(citizenID);

                        if (company.CompanyManagers.Any(manager => manager.EntityID == citizenID))
                            RejectVoting(voting, CongressVotingRejectionReasonEnum.AlreadyManagerAssignManager);

                        companyService.AddManager(company, citizen, new CompanyRights(true, 100));
                        break;
                    }
                case VotingTypeEnum.TransferCashToCompany:
                    {
                        decimal amount = decimal.Parse(voting.Argument1);
                        int companyID = int.Parse(voting.Argument2);
                        int currencyID = int.Parse(voting.Argument3);

                        var company = companyRepository.GetById(companyID);
                        var money = new Money(currencyID, amount);
                        var country = voting.Country;

                        var possibleReason = GetRejectionReasonForTransferCashToCompany(money, company, country);

                        if (possibleReason.HasValue)
                        {
                            RejectVoting(voting, possibleReason.Value);
                            break;
                        }

                        transactionsService.TransferMoneyFromCountryToCompany(money, company, country);
                        RemoveFromGameMoneyInVoting(voting);

                        break;
                    }
                case VotingTypeEnum.ChangeGreetingMessage:
                    {
                        int greetingID = int.Parse(voting.Argument1);
                        var message = votingGreetingMessageRepository.GetById(greetingID).Message;


                        voting.Country.GreetingMessage = message;
                        break;
                    }
                case VotingTypeEnum.ChangeCitizenStartingMoney:
                    {
                        decimal amount = decimal.Parse(voting.Argument1);

                        var policy = voting.Country.CountryPolicy;
                        policy.CitizenFee = amount;
                        break;
                    }
                case VotingTypeEnum.ChangeMinimumContractLength:
                    {
                        int length = int.Parse(voting.Argument1);

                        var policy = voting.Country.CountryPolicy;
                        policy.MinimumContractLength = length;
                        break;
                    }
                case VotingTypeEnum.ChangeMaximumContractLength:
                    {
                        int length = int.Parse(voting.Argument1);
                        var policy = voting.Country.CountryPolicy;
                        policy.MaximumContractLength = length;
                        break;
                    }
                case VotingTypeEnum.PrintMoney:
                    {
                        int moneyAmount = int.Parse(voting.Argument1);

                        var money = new Money()
                        {
                            Currency = Persistent.Countries.GetCountryCurrency(voting.CountryID),
                            Amount = moneyAmount
                        };

                        transactionsService.PrintMoney(voting.Country, money);



                        RemoveFromGameMoneyInVoting(voting);
                        break;
                    }
                case VotingTypeEnum.ChangeMinimalWage:
                    {
                        var minimalWage = decimal.Parse(voting.Argument1);

                        voting.Country.CountryPolicy.MinimalWage = minimalWage;
                        companyService.RemoveJobOffersThatDoesNotMeetMinimalWage(minimalWage, voting.CountryID);

                        break;
                    }
                case VotingTypeEnum.BuildDefenseSystem:
                    {
                        var quality = int.Parse(voting.Argument3);
                        var regionID = int.Parse(voting.Argument1);

                        var region = regionRepository.GetById(regionID);
                        defenseSystemService.BuildDefenseSystem(region, voting.Country, quality, constructionService);
                        break;

                    }
                case VotingTypeEnum.ChangeHotelTax:
                    {
                        var tax = decimal.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.HotelTax = tax / 100m;
                        break;
                    }
                case VotingTypeEnum.ChangeHouseTax:
                    {
                        var tax = decimal.Parse(voting.Argument1);
                        voting.Country.CountryPolicy.HouseTax = tax / 100m;
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            countryEventService.AddVotingEvent(voting, VotingStatusEnum.Accepted);
            ConditionalSaveChanges(congressVotingRepository);
        }

        private void addForeignTax(CongressVoting voting, ProductTypeEnum productType, double rate, int foreignCountryID)
        {
            var tax = productTaxRepository.FirstOrDefault(t => t.ProductID == (int)productType &&
            t.CountryID == voting.CountryID);

            if (tax == null)
            {
                tax = new ProductTax()
                {
                    CountryID = voting.CountryID,
                    ProductID = (int)productType,
                    ProductTaxTypeID = (int)getProductTaxType((VotingTypeEnum)voting.VotingTypeID),
                    TaxRate = (decimal)(rate / 100.0),
                    ForeignCountryID = foreignCountryID
                };
                productTaxRepository.Add(tax);
            }
            else
                tax.TaxRate = (decimal)rate;
        }

        private ProductTaxTypeEnum getProductTaxType(VotingTypeEnum votingType)
        {
            switch (votingType)
            {
                case VotingTypeEnum.ChangeProductVAT:
                    return ProductTaxTypeEnum.VAT;
                case VotingTypeEnum.ChangeProductImportTax:
                    return ProductTaxTypeEnum.Import;
                case VotingTypeEnum.ChangeProductExportTax:
                    return ProductTaxTypeEnum.Export;
            }

            throw new NotImplementedException();
        }


        public List<CongressVote> CreateAbstainedVotes(CongressVoting voting, IEnumerable<Congressman> abstainedCongressmen)
        {
            var votes = new List<CongressVote>();
            foreach (var congressman in abstainedCongressmen)
            {
                CongressVote vote = new CongressVote()
                {
                    CitizenID = congressman.CitizenID,
                    CongressVotingID = voting.ID,
                    VoteTypeID = (int)VoteTypeEnum.Abstained
                };

                votes.Add(vote);
            }

            return votes;
        }

        public void ReserveMoneyForVoting(CongressVoting voting, params Money[] money)
        {
            foreach (var m in money)
            {
                var reservedMoney = new CongressVotingReservedMoney()
                {
                    Amount = m.Amount,
                    CurrencyID = m.Currency.ID
                };

                voting.CongressVotingReservedMoneys.Add(reservedMoney);

                var country = Persistent.Countries.GetById(voting.CountryID);

                var transaction = new structs.Transaction()
                {
                    Arg1 = "ReserveCongressMoney",
                    Arg2 = string.Format("country {0} - {1} {2}", country.Entity.Name, m.Amount, m.Currency.Symbol),
                    DestinationEntityID = null,
                    Money = m,
                    SourceEntityID = voting.CountryID,
                    TransactionType = TransactionTypeEnum.ReserveCongressVotingReservedMoney
                };

                transactionsService.MakeTransaction(transaction);
            }

            ConditionalSaveChanges(congressVotingReservedMoneyRepository);
        }

        public void UnreserveMoneyForVoting(CongressVoting voting)
        {
            var reservedMoney = voting.CongressVotingReservedMoneys.ToList();
            foreach (var reserved in reservedMoney)
            {
                var money = new Money(reserved.CurrencyID, reserved.Amount);

                var transaction = new structs.Transaction()
                {
                    Arg1 = "ReserveCongressMoney",
                    Arg2 = string.Format("country {0} - {1} {2} unreserved in #{3}", voting.Country.Entity.Name, money.Amount, money.Currency.Symbol, voting.ID),
                    DestinationEntityID = voting.CountryID,
                    Money = money,
                    SourceEntityID = null,
                    TransactionType = TransactionTypeEnum.ReserveCongressVotingReservedMoney
                };

                transactionsService.MakeTransaction(transaction);
                congressVotingReservedMoneyRepository.Remove(reserved);
            }

            ConditionalSaveChanges(congressVotingReservedMoneyRepository);
        }

        public void RemoveFromGameMoneyInVoting(CongressVoting voting)
        {
            var reservedMoney = congressVotingReservedMoneyRepository.GetReservedMoneyForVoting(voting.ID);

            foreach (var reserved in reservedMoney)
                congressVotingReservedMoneyRepository.Remove(reserved);

            ConditionalSaveChanges(congressVotingReservedMoneyRepository);
        }

        public MethodResult CanTransferCashToCompany(Money money, Company destination, Country source)
        {
            if (destination == null)
                return new MethodResult("Company does not exist!");
            if (source == null)
                return new MethodResult("Country does not exist!");
            if (money.Amount <= 0)
                return new MethodResult($"You must send at least 0.01 {money.Currency.Symbol}!");

            var possibleError = GetRejectionReasonForTransferCashToCompany(money, destination, source);

            if (possibleError != null)
                return new MethodResult(possibleError.Value.ToHumanReadable().FirstUpper());

            return MethodResult.Success;
        }

        public CongressVotingRejectionReasonEnum? GetRejectionReasonForTransferCashToCompany(Money money, Company destination, Country source)
        {
            if (destination.Region.CountryID != source.ID)
                return CongressVotingRejectionReasonEnum.CompanyOutsideNationalBordersTransferMoney;

            return null;
        }

        public MethodResult CanCreateVotingWithMinimumContractLength(int countryID, int minimumLength)
        {
            if (minimumLength < Constants.ContractMinimumLength)
                return new MethodResult($"Contract must have length of at least {Constants.ContractMinimumLength} days!");

            if (minimumLength > Constants.ContractMaximumLength)
                return new MethodResult($"Contract must have length at most {Constants.ContractMaximumLength} days!");

            var countryMaxLength = countryRepository.GetCountryPolicySetting(countryID, p => p.MaximumContractLength);

            if (minimumLength > countryMaxLength)
                return new MethodResult($"You cannot have minimum contract length which is longer than maximum length({countryMaxLength})!");

            var maxActiveVotings = congressVotingRepository.GetNotFinishedVotings(countryID, VotingTypeEnum.ChangeMaximumContractLength);

            if (maxActiveVotings.Any())
            {
                var lengths = maxActiveVotings.Select(v => int.Parse(v.Argument1));
                if (lengths.Any(l => l < minimumLength))
                    return new MethodResult("There is already maximum contract length active voting which have length lower than this voting!");
            }

            return MethodResult.Success;
        }

        public MethodResult CanCreateVotingWithMaximumContractLength(int countryID, int maximumLength)
        {
            if (maximumLength < Constants.ContractMinimumLength)
                return new MethodResult($"Contract must have length of at least {Constants.ContractMinimumLength} days!");

            if (maximumLength > Constants.ContractMaximumLength)
                return new MethodResult($"Contract must have length at most {Constants.ContractMaximumLength} days!");

            var countryMinLength = countryRepository.GetCountryPolicySetting(countryID, p => p.MinimumContractLength);

            if (maximumLength < countryMinLength)
                return new MethodResult($"You cannot have maximum contract length which is lower than minimum length({countryMinLength})!");

            var maxActiveVotings = congressVotingRepository.GetNotFinishedVotings(countryID, VotingTypeEnum.ChangeMinimumContractLength);

            if (maxActiveVotings.Any())
            {
                var lengths = maxActiveVotings.Select(v => int.Parse(v.Argument1));
                if (lengths.Any(l => l > maximumLength))
                    return new MethodResult("There is already minimum contract length active voting which have length bigger than this voting!");
            }

            return MethodResult.Success;
        }
    }
}
