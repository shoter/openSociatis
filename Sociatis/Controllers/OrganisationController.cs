using Common.Operations;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models;
using Sociatis.Models.Organisation;
using Sociatis.Validators.Organisations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs;

namespace Sociatis.Controllers
{
    public class OrganisationController : ControllerBase
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly ICountryRepository countryRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IEntityRepository entityRepository;
        private readonly IOrganisationRepository organisationRepository;
        private readonly IOrganisationService organisationService;
        private readonly ITransactionsService transactionService;
        private readonly IWalletRepository walletRepository;
        
        public OrganisationController(IConfigurationRepository configurationRepository, ICountryRepository countryRepository, 
            ICurrencyRepository currencyRepository, IOrganisationService organisationService, IEntityRepository entityRepository,
            IOrganisationRepository organisationRepository, ITransactionsService transactionService, IPopupService popupService,
            IWalletRepository walletRepository) : base(popupService)
        {
            this.configurationRepository = configurationRepository;
            this.countryRepository = countryRepository;
            this.currencyRepository = currencyRepository;
            this.organisationService = organisationService;
            this.entityRepository = entityRepository;
            this.organisationRepository = organisationRepository;
            this.transactionService = transactionService;
            this.walletRepository = walletRepository;
        } 

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        public ActionResult Create()
        {
            CreateOrganisationViewModel vm = new CreateOrganisationViewModel();
            PrepareViewModel(vm);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Create(CreateOrganisationViewModel vm)
        {
            PrepareViewModel(vm);
            var validator = new OrganisationCreationValidator(ModelState, currencyRepository, entityRepository, organisationRepository);
            
            if (validator.Validate(vm, SessionHelper.CurrentEntity))
            {
                var param = new CreateOrganisationParameters()
                {
                    Name = vm.OrganisationName,
                    OwnerID = SessionHelper.CurrentEntity.EntityID,
                    CountryID = vm.CountryID
                };

                var org = organisationService.CreateOrganisation(param);

                Transaction adminTransaction, countryTransaction;
                prepareTransactions(vm, SessionHelper.CurrentEntity, org, org.Country, out adminTransaction, out countryTransaction);

                transactionService.MakeTransaction(adminTransaction);
                transactionService.MakeTransaction(countryTransaction);

                return RedirectToHome();
            }

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Organisation/{organisationID:int}/Inventory")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation)]
        public ActionResult Inventory(int organisationID)
        {
            var organisation = organisationRepository.GetById(organisationID);

            MethodResult result = organisationService.CanSeeInventory(SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen, organisation);
            if (result.IsError)
                return RedirectToHomeWithError(result);

            var vm = new OrganisationEquipmentViewModel(organisation, organisation.Entity.Equipment);

            return View(vm);
        }

        [Route("Organisation/{organisationID:int}/Wallet")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Wallet(int organisationID)
        {
            var organisation = organisationRepository.GetById(organisationID);


            MethodResult result = organisationService.CanSeeWallet(SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen, organisation);
            if (result.IsError)
                return RedirectToHomeWithError(result);

            var walletID = organisation.Entity.WalletID;
            var money = walletRepository.GetMoney(walletID).ToList();

            var vm = new OrganisationWalletViewModel(organisation, money);

            return View(vm);

        }

        private void prepareTransactions(CreateOrganisationViewModel vm, Entities.Entity entity, Entities.Organisation organisation, Entities.Country country, out Transaction adminTransaction, out Transaction countryTransaction)
        {

            var adminCurrency = currencyRepository.GetById(vm.AdminFee.CurrencyID);
            var countryCurrency = currencyRepository.GetById(vm.CountryFee.CurrencyID);

            var adminMoney = new Money()
            {
                Amount = vm.AdminFee.Quantity,
                Currency = adminCurrency
            };

            var countryMoney = new Money()
            {
                Amount = vm.CountryFee.Quantity,
                Currency = countryCurrency
            };

            adminTransaction = new Transaction()
            {
                Arg1 = "Admin Fee",
                Arg2 = string.Format("{0}({1}) created organisation {2}({3})", entity.Name, entity.EntityID, organisation.Entity.Name, organisation.ID),
                DestinationEntityID = null,
                Money = adminMoney,
                SourceEntityID = entity.EntityID,
                TransactionType = TransactionTypeEnum.CompanyCreate
            };
            countryTransaction = new Transaction()
            {
                Arg1 = "Country Fee",
                Arg2 = adminTransaction.Arg2,
                DestinationEntityID = country.ID,
                Money = countryMoney,
                SourceEntityID = entity.EntityID,
                TransactionType = TransactionTypeEnum.CompanyCreate
            };
        }

        private bool isValid(CreateOrganisationViewModel vm)
        {
            throw new NotImplementedException();
        }

        private void PrepareViewModel(CreateOrganisationViewModel vm)
        {
            var citizen = SessionHelper.CurrentEntity.Citizen;
            var country = citizen.Region.Country;

            vm.CountryID = country.ID;
            vm.CountryName = country.Entity.Name;

            vm.CountryFee = new MoneyViewModel(country.Currency, country.CountryPolicy.OrganisationCreateCost);
            vm.AdminFee = new MoneyViewModel(currencyRepository.Gold, configurationRepository.First().OrganisationCreationFee);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Organisation/{organisationID:int}")]
        public ViewResult View(int organisationID)
        {
            var organisation = organisationRepository.GetById(organisationID);
            var vm = new OrganisationViewModel(organisation);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Organisation/")]
        public ViewResult List()
        {
            var entity = SessionHelper.CurrentEntity;
            var organisations = organisationRepository
                .Where(o => o.OwnerID == entity.EntityID)
                .Include(o => o.Entity)
                .ToList();

            var vm = new OrganisationListViewModel(organisations);

            return View(vm);
        }


    }
}