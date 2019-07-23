using Common;
using Common.Exceptions;
using Common.Operations;
using Common.Transactions;
using Entities;
using Entities.enums;
using Entities.QueryEnums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Constructions;
using Sociatis.Models.Country;
using Sociatis.Models.President;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs;
using WebUtils;
using WebUtils.Attributes;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository countryRepository;
        private readonly ICountryService countryService;
        private readonly IPresidentVotingRepository presidentVotingRepository;
        private readonly IWarService warService;
        private readonly ITransactionsService transactionService;
        private readonly IWarRepository warRepository;
        private readonly IRegionService regionService;
        private readonly IRegionRepository regionRepository;
        private readonly IEmbargoService embargoService;
        private readonly IEmbargoRepository embargoRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ICountryTreasureService countryTreasureService;
        private readonly IWalletService walletService;
        private readonly ICompanyService companyService;
        private readonly IMPPService mppService;
        private readonly IConstructionRepository constructionRepository;
        private readonly IMilitaryProtectionPactRepository mppRepository;
        private readonly IMilitaryProtectionPactOfferRepository mppOfferRepository;
        private readonly ITransactionScopeProvider transactionScopeProvider;
        private readonly IConstructionService constructionService;

        public CountryController(ICountryRepository countryRepository, ICountryService countryService, IPresidentVotingRepository presidentVotingRepository, IWarService warService,
            ITransactionsService transactionService, IWarRepository warRepository, IRegionService regionService, IRegionRepository regionRepository, IEmbargoService embargoService,
            IEmbargoRepository embargoRepository, IWalletRepository walletRepository, IPopupService popupService, ICountryTreasureService countryTreasureService,
            IWalletService walletService, ICompanyService companyService, IMPPService mppService, IMilitaryProtectionPactRepository mppRepository,
            IMilitaryProtectionPactOfferRepository mppOfferRepository, ITransactionScopeProvider transactionScopeProvider, IConstructionRepository constructionRepository,
            IConstructionService constructionService) : base(popupService)
        {
            this.countryRepository = countryRepository;
            this.countryService = countryService;
            this.presidentVotingRepository = presidentVotingRepository;
            this.warService = warService;
            this.transactionService = transactionService;
            this.warRepository = warRepository;
            this.regionService = regionService;
            this.regionRepository = regionRepository;
            this.embargoService = embargoService;
            this.embargoRepository = embargoRepository;
            this.walletRepository = walletRepository;
            this.countryTreasureService = countryTreasureService;
            this.walletService = walletService;
            this.companyService = companyService;
            this.mppService = mppService;
            this.mppRepository = mppRepository;
            this.mppOfferRepository = mppOfferRepository;
            this.transactionScopeProvider = transactionScopeProvider;
            this.constructionRepository = constructionRepository;
            this.constructionService = constructionService;
        }

        [Route("Country/{countryID:int}/Wallet")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Wallet(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            if (country == null)
                return RedirectToHomeWithError("Country does not exist");

            var walletID = country.Entity.WalletID;
            var money = walletRepository.GetMoney(walletID).ToList();

            var vm = new CountryWalletViewModel(country, money);

            return View(vm);

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Constructions")]
        public ActionResult NationalConstructions(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            var constructions = constructionRepository.GetNationalConstructions(country.ID);
            var vm = new NationalConstructionsViewModel(country, constructions, constructionService);

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/President")]
        public ActionResult President(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
            {
                return redirectNoCountry();
            }

            var vm = new PresidentViewModel(country, countryService, countryRepository);

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Country/{countryID}/ConstructCompany")]
        public ActionResult ConstructCompany(int countryID)
        {
            var entity = SessionHelper.CurrentEntity;
            var citizen = SessionHelper.LoggedCitizen;
            var country = countryRepository.GetById(countryID);

            MethodResult result;
            if ((result = countryService.CanCreateCountryCompany(entity, country, citizen)).IsError)
                return RedirectBackWithError(result);

            var vm = new ConstructCompanyViewModel(country, regionRepository, walletService, countryTreasureService);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Country/{countryID:int}/ManageSpawn")]
        public ActionResult ManageSpawn(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            throw new NotImplementedException();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Country/{countryID}/ConstructCompany")]
        public ActionResult ConstructCompany(ConstructCompanyViewModel vm)
        {
            var country = countryRepository.GetById(vm.CountryID);
            var entity = SessionHelper.CurrentEntity;
            var citizen = SessionHelper.LoggedCitizen;
            if (ModelState.IsValid)
            {
                MethodResult result;
                var region = regionRepository.GetById(vm.RegionID);
                if ((result = countryService.CanCreateCountryCompany(vm.CompanyName, entity, country, citizen, region, (ProductTypeEnum)vm.ProductID)).IsError)
                    AddError(result);
                else
                {
                    Company company;
                    using (var scope = transactionScopeProvider.CreateTransactionScope())
                    {
                        company = companyService.CreateCompanyForCountry(vm.CompanyName, (ProductTypeEnum)vm.ProductID, vm.RegionID, country);
                        companyService.AddManager(company, citizen, CompanyRights.FullRgihts);

                        scope?.Complete();
                    }
                    AddSuccess("Company has been created successfully!");
                    return RedirectToAction("View", "Company", new { companyID = company.ID });
                }
            }

            vm = new ConstructCompanyViewModel(country, regionRepository, walletService, countryTreasureService);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Country/{countryID:int}/DeclareEmbargo")]

        public ActionResult DeclareEmbargo(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (embargoService.CanDeclareEmbargo(country, SessionHelper.CurrentEntity) == false)
                return redirectCannotDeclareEmbargo(countryID);

            var possibleEmbargoes = countryRepository.Where(
                c => c.Embargoes.Any(e => e.CreatorCountryID == countryID) == false && c.ID != countryID)
                .Select(c => c.Entity)
                .ToList();

            var vm = new DeclareEmbargoViewModel(country, possibleEmbargoes);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Country/{countryID:int}/DeclareEmbargo")]
        public ActionResult DeclareEmbargo(DeclareEmbargoViewModel vm, int countryID)
        {
            var issuingCountry = countryRepository.GetById(countryID);
            var embargoedCountry = countryRepository.GetById(vm.CountryEmbargoID);

            if (embargoService.CanDeclareEmbargo(issuingCountry, embargoedCountry, SessionHelper.CurrentEntity) == false)
            {
                return redirectCannotDeclareEmbargo(countryID);
            }

            var haveMoney = embargoService.CanUpkeepEmbargo(issuingCountry, embargoedCountry);

            if (haveMoney)
            {
                embargoService.DeclareEmbargo(issuingCountry, embargoedCountry);

                AddSuccess("Embargo was successfully created!");
                return RedirectToAction("Embargoes", new { countryID = countryID });
            }
            else
            {
                AddError("Your country does not have enough moeny to do that");
                return RedirectToAction("DeclareEmbargo", new { countryID = countryID });
            }

        }

        private ActionResult redirectCannotDeclareEmbargo(int countryID)
        {
            AddError("You cannot declare this embargo!");
            return RedirectToAction("Embargoes", new { countryID = countryID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public PartialViewResult GetEmbargoCost(int declareCountryID, int CountryEmbargoID)
        {
            var declareCountry = countryRepository.GetById(declareCountryID);
            var embargoedCountry = countryRepository.GetById(CountryEmbargoID);

            var cost = embargoService.GetEmbargoCost(declareCountry, embargoedCountry);
            bool haveMoney = embargoService.CanUpkeepEmbargo(cost, declareCountry.Entity.WalletID);

            var vm = new EmbargoCostViewModel(declareCountry, embargoedCountry, cost, haveMoney);

            return PartialView(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult CancelEmbargo(int embargoID)
        {
            var embargo = embargoRepository.GetById(embargoID);
            if (embargo == null)
                return RedirectToHomeWithError("Embargo does not exist!");

            if (embargoService.CanCancelEmbargo(embargo, SessionHelper.CurrentEntity) == false)
            {
                AddError("You cannot cancel this embargo due to insufficient rights.");
                return RedirectToAction("Embargoes", new { countryID = embargo.CreatorCountryID });
            }

            embargoService.CancelEmbargo(embargo);

            AddSuccess("Embargo was canceled.");
            return RedirectToAction("Embargoes", new { countryID = embargo.CreatorCountryID });
        }


        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/embargoes")]
        public ActionResult Embargoes(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
                return redirectNoCountry();

            var vm = new EmbargoesPageViewModel(country, embargoService);

            return View(vm);
        }


        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}")]
        public ActionResult View(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            if (country == null)
                return redirectNoCountry();

            var vm = new IndexCountryViewModel(country);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/President/SendMPPOffer")]
        [HttpGet]
        public ActionResult SendMPPOffer(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            var entity = SessionHelper.CurrentEntity;

            var result = mppService.CanOfferMPP(entity, country);
            if (result.IsError)
                return RedirectBackWithError(result);
            var vm = new OfferMPPViewModel(country, mppService, walletService);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult PostSendMPPOffer(int countryID, int otherCountryID, int days)
        {
            var country = countryRepository.GetById(countryID);
            var otherCountry = countryRepository.GetById(otherCountryID);
            var entity = SessionHelper.CurrentEntity;

            var result = mppService.CanOfferMPP(entity, country, otherCountry, days);
            if (result.IsError)
                return RedirectBackWithError(result);

            mppService.OfferMPP(entity, country, otherCountry, days);

            AddSuccess("MPP offer sent!");
            return RedirectToAction(nameof(this.MPPs), new { countryID = countryID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/MPPs")]
        [HttpGet]
        public ActionResult MPPs(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectBackWithError("Country does not exist!");

            var mpps = mppRepository.Where(c =>
(c.FirstCountryID == countryID || c.SecondCountryID == countryID) && c.Active).ToList();

            var proposed = mppOfferRepository.Where(c => c.FirstCountryID == countryID).ToList();
            var proposals = mppOfferRepository.Where(c => c.SecondCountryID == countryID).ToList();

            var vm = new CountryMPPListViewModel(country, mpps, proposed, proposals);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/President/DeclareWar")]
        public ActionResult DeclareWar(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectToHomeWithError("Country does not exist");



            var entityID = SessionHelper.CurrentEntity.EntityID;

            if (isPresident(country, entityID) == false)
                return RedirectToHomeWithError("You are not a president of this country to declare wars in it's behalf!");

            var regions = country.Regions.ToList();

            var vm = new DeclareWarViewModel(country, countryRepository);

            return View(vm);
        }

        private static bool isPresident(Entities.Country country, int entityID)
        {
            return entityID == country.PresidentID || entityID == country.ID;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{AttackerCountryID:int}/President/DeclareWar")]
        [HttpPost]
        public ActionResult DeclareWar([System.Web.Http.FromBody]WarDeclarationViewModel vm)
        {
            var defender = countryRepository.GetById(vm.SelectedCountryID);

            if (defender == null)
                return RedirectToHomeWithError("Country does not exist!");

            var attacker = countryRepository.GetById(vm.AttackerCountryID);

            if (attacker == null)
                return RedirectToHomeWithError("Country does not exist!");

            if (isPresident(attacker, SessionHelper.CurrentEntity.EntityID) == false)
                return RedirectToHomeWithError("You are not a president!");


            var result = warService.CanDeclareWar(attacker, defender);

            if (result.IsError)
            {
                AddError(result);
                return RedirectBack();
            }

            if (ModelState.IsValid)
            {
                var warID = (int)warService.DeclareWar(attacker, defender);
                AddInfo("War has been declared!");

                return RedirectToAction("View", "War", new { warID = warID });
            }

            var viewVm = new DeclareWarViewModel(attacker, countryRepository);

            return View(viewVm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult GetWarDeclareInformation(int attackerID, int defenderID)
        {
            try
            {
                var attacker = countryRepository.GetById(attackerID);
                var defender = countryRepository.GetById(defenderID);

                if (attacker == null || defender == null)
                    throw new UserReadableException("Country does not exist!");

                var goldNeeded = warService.GetNeededGoldToStartWar(attacker, defender);
                var attackerAllies = countryRepository.GetAllies(attackerID);
                var defenderAllies = countryRepository.GetAllies(defenderID);

                var vm = new DeclareWarInformationsViewModel(attacker, goldNeeded, attackerAllies, defenderAllies);

                return PartialView(vm);
            }
            catch (UserReadableException e)
            {
                throw;
            }
            catch (Exception)
            {
                return Content("");
            }
        }

        private ActionResult redirectNoCountry()
        {
            return RedirectToHomeWithError("Country does not exist!");
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/President/Candidates")]
        public ActionResult PresidentCandidates(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
            {
                return redirectNoCountry();
            }

            var voting = country.PresidentVotings.Last();
            MethodResult canCandidate = MethodResult.Success;
            bool canVoteOnPresident = false;
            var entity = SessionHelper.CurrentEntity;

            if (entity.Citizen != null)
            {
                canCandidate = countryService.CanCandidateAsPresident(entity.Citizen, country);
                canVoteOnPresident = countryService.CanVoteInPresidentElections(entity.Citizen, voting).isSuccess;
            }

            var vm = new PresidentCandidatesListViewModel(voting, canCandidate, canVoteOnPresident);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Geography")]
        public ActionResult Geography(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
            {
                return redirectNoCountry();
            }

            var vm = new CountryRegionsListViewModel(country);

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult WarsPost(int countryID, PagingParam pagingParam)
        {
            return RedirectToAction("Wars", new Dictionary<string, object> { { "countryID", countryID }, { "pagingParam.PageNumber", pagingParam.PageNumber } });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Wars/{pagingParam.PageNumber:int=1}")]
        [HttpGet]
        public ActionResult Wars(int countryID, PagingParam pagingParam)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var country = countryRepository.GetById(countryID);
            if (country == null)
                return redirectNoCountry();

            var wars = warRepository.GetAllWarsForCountry(countryID, WarActivitySearchCriteria.Both);
            wars = wars.OrderBy(w => w.EndDay == null).OrderByDescending(w => w.ID);

            var vm = new CountryWarsViewModel(wars, country, pagingParam, warService);

            return View(vm);

        }
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CandidateAsPresident(int countryID)
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity.Citizen == null)
                return RedirectBackWithError("You are not a citizen");

            var citizen = entity.Citizen;
            var country = countryRepository.GetById(countryID);

            if (country == null)
            {
                return redirectNoCountry();
            }

            MethodResult result;
            if ((result = countryService.CanCandidateAsPresident(citizen, country)).IsError)
                return RedirectBackWithError(result);

            countryService.CandidateAsPresident(citizen, country);

            AddInfo("You are candidate to next president elections!");

            return RedirectBack();
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult VoteOnPresident(int candidateID)
        {
            var candidate = presidentVotingRepository.GetCandidateByID(candidateID);

            if (candidate == null)
                return RedirectToHomeWithError("Candidate does not exist!");

            var entity = SessionHelper.CurrentEntity;

            if (entity.Citizen == null)
                return RedirectBackWithError("You cannot vote");

            MethodResult result;
            if ((result = countryService.CanVoteOnPresidentCandidate(entity.Citizen, candidate)).IsError)
                return RedirectBackWithError(result);

            countryService.VoteOnPresidentCandidate(entity.Citizen, candidate);
            AddInfo(string.Format("You successfully voted on {0}", candidate.Citizen.Entity.Name));
            return RedirectBack();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Treasury")]
        public ActionResult Treasury(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            var entity = SessionHelper.CurrentEntity;

            MethodResult result;
            if ((result = countryTreasureService.CanSeeCountryTreasure(country, entity)).IsError)
                return RedirectBackWithError(result);

            var money = walletRepository.GetMoneyForEntity(country.ID);

            var vm = new CountryTreasureViewModel(country, money);

            return View(vm);
        }


        public JsonResult GetMPPAvailableCountries(Select2Request request, int countryID)
        {
            string search = request.Query.Trim().ToLower();

            var query = countryRepository
                .Where(country => 
                country.ID != countryID &&
                country.MPPOffersFirstCountry.Any(o => o.SecondCountryID == countryID) == false &&
                country.MPPOffersSecondCountry.Any(o => o.FirstCountryID == countryID) == false &&
                country.MPPsFirstCountry.Any(mpp => mpp.FirstCountryID == countryID && mpp.Active) == false &&
                country.MPPsSecondCountry.Any(mpp => mpp.SecondCountryID == countryID && mpp.Active) == false &&
                country.Entity.Name.ToLower().Contains(search) &&
                country.AttackerWars.Any(w => w.DefenderCountryID == countryID && w.Active) == false &&
                country.DefenderWars.Any(w => w.AttackerCountryID == countryID && w.Active) == false)
                .OrderBy(c => c.Entity.Name)
                .Select(c => new Select2Item()
                {
                    id = c.ID,
                    text = c.Entity.Name
                });

            return Select2Response(query, request);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CancelProposal(int offerID)
        {
            var offer = mppOfferRepository.GetById(offerID);
            var entity = SessionHelper.CurrentEntity;

            var result = mppService.CanCancelMPP(offer, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            mppService.CancelMPP(offer);
            return RedirectBackWithSuccess("MPP proposition has been canceled!");
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult AcceptProposal(int offerID)
        {
            var offer = mppOfferRepository.GetById(offerID);
            var entity = SessionHelper.CurrentEntity;

            var result = mppService.CanAcceptMPP(offer, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            mppService.AcceptMPP(offer);
            return RedirectBackWithSuccess("MPP proposition has been accepted!");
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult RefuseProposal(int offerID)
        {
            var offer = mppOfferRepository.GetById(offerID);
            var entity = SessionHelper.CurrentEntity;

            var result = mppService.CanRefuseMPP(offer, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            mppService.RefuseMPP(offer);
            return RedirectBackWithSuccess("MPP proposition has been refused!");
        }


        [HttpPost]
        [AjaxOnly]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult GetGoldCostForMPPOffer(int? firstCountryID, int? secondCountryID, int length)
        {
            if (firstCountryID.HasValue == false || secondCountryID.HasValue == false)
                return JsonData(null);
            try
            {
                var cost = mppService.CalculateMPPCost(length);
                return JsonData(cost);
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }
        }

    }
}