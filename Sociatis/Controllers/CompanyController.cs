using Entities.enums;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using Entities.Extensions;
using Sociatis.Models;
using Sociatis.Validators;
using WebServices.structs;
using Entities;
using System.Diagnostics;
using WebServices.BigParams.jobOffers;
using WebServices.BigParams.Company;
using System.Data.Entity;
using WebUtils.Attributes;
using Sociatis.Code.Filters;
using Sociatis.ActionFilters;
using WebUtils;
using WebServices.BigParams.MarketOffers;
using Sociatis.Validators.Company;
using Common.Operations;
using WebServices.enums;
using WebServices.Models;
using WebUtils.Forms.Select2;
using Sociatis.Models.Companies.Managers;
using WebServices.structs.Companies;
using WebServices.Companies;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class CompanyController : ControllerBase
    {
        ICompanyService companyService;
        ICompanyRepository companyRepository;
        IConfigurationRepository configurationRepository;
        ICurrencyRepository currencyRepository;
        ITransactionsService transactionService;
        WebServices.IWalletService walletService;
        IJobOfferService jobOfferService;
        IJobOfferRepository jobOfferRepository;
        IProductRepository productRepository;
        ICompanyEmployeeRepository companyEmployeeRepository;
        IRegionRepository regionRepository;
        IMarketOfferRepository marketOfferRepository;
        IEquipmentRepository equipmentRepository;
        IMarketService marketService;
        ICountryRepository countryRepository;
        IEntityRepository entityRepository;
        IEmployeeService employeeService;
        ICompanyManagerRepository companyManagerRepository;
        IContractService contractService;
        ICitizenRepository citizenRepository;
        private readonly Entities.Repository.IWalletRepository walletRepository;
        private readonly IRegionService regionService;
        private readonly IHospitalRepository hospitalRepository;
        private readonly ICompanyFinanceSummaryService companyFinanceSummaryService;

        public CompanyController(IJobOfferRepository jobOfferRepository, IProductRepository productRepository, ICompanyEmployeeRepository companyEmployeeRepository,
            ICurrencyRepository currencyRepository, WebServices.IWalletService walletService, IJobOfferService jobOfferService,
            ICompanyService companyService, ICompanyRepository companyRepository, IConfigurationRepository configurationRepository, ITransactionsService transactionService,
            IRegionRepository regionRepository, IMarketOfferRepository marketOfferRepository, IEquipmentRepository equipmentRepository,
            IMarketService marketService, ICountryRepository countryRepository, IEntityRepository entityRepository, IEmployeeService employeeService,
            IContractService contractService, Entities.Repository.IWalletRepository walletRepository, IPopupService popupService, ICompanyManagerRepository companyManagerRepository,
            ICitizenRepository citizenRepository, IRegionService regionService, IHospitalRepository hospitalRepository,
            ICompanyFinanceSummaryService companyFinanceSummaryService) : base(popupService)
        {
            this.companyService = companyService;
            this.companyRepository = companyRepository;
            this.configurationRepository = configurationRepository;
            this.currencyRepository = currencyRepository;
            this.transactionService = transactionService;
            this.walletService = walletService;
            this.jobOfferService = jobOfferService;
            this.jobOfferRepository = jobOfferRepository;
            this.productRepository = productRepository;
            this.companyEmployeeRepository = companyEmployeeRepository;
            this.regionRepository = regionRepository;
            this.marketOfferRepository = marketOfferRepository;
            this.equipmentRepository = equipmentRepository;
            this.marketService = marketService;
            this.countryRepository = countryRepository;
            this.entityRepository = entityRepository;
            this.employeeService = employeeService;
            this.contractService = contractService;
            this.walletRepository = walletRepository;
            this.companyManagerRepository = companyManagerRepository;
            this.citizenRepository = citizenRepository;
            this.regionService = regionService;
            this.hospitalRepository = hospitalRepository;
            this.companyFinanceSummaryService = companyFinanceSummaryService;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [IsCitizen]
        [Route("Company/Create")]
        public ActionResult Create()
        {
            if (!canCreateCompany())
                return RedirectToAction("Index", "Business");

            CreateCompanyViewModel vm = new CreateCompanyViewModel();

            prepareCreateCompanyViewModel(vm);


            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult UpgradeCompany(int companyID)
        {
            try
            {
                var entity = SessionHelper.CurrentEntity;
                var citizen = SessionHelper.LoggedCitizen;
                var company = companyRepository.GetById(companyID);

                var result = companyService.CanUpgradeCompany(company, entity, citizen);
                if (result.IsError)
                    return JsonError(result);

                companyService.UpgradeCompany(company);

                var msg = $"Company has been upgraded to Q{company.Quality}";
                return JsonSuccess(msg);
            }
            catch (Exception)
            {
                return JsonError("Undefined error");
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [IsOrganisation]
        [Route("Company/Create")]
        public ActionResult CreateAsOrganisation()
        {
            if (!canCreateCompany())
                return RedirectToAction("Index", "Home");

            var vm = new CreateCompanyAsOrganisationViewModel();

            prepareCreateCompanyViewModel(vm);

            return View("CreateAsOrganisation", vm);
        }



        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult DeleteJobOffer(int jobOfferID)
        {
            var jobOffer = jobOfferRepository.GetById(jobOfferID);
            if (jobOffer == null)
                return RedirectBackWithError("Job offer does not exist!");
            var company = jobOffer.Company;

            var companyRights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (companyRights.CanPostJobOffers == false)
                return RedirectBackWithError("You cannot do that!");

            jobOfferRepository.Remove(jobOffer);
            jobOfferRepository.SaveChanges();

            AddSuccess("You successfully deleted job offer.");
            return RedirectBack();
        }

        [Route("Company/{companyID:int}/Wallet")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Wallet(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            if (company == null)
                return RedirectToHomeWithError("Company does not exist!");

            if (companyService.DoesHaveRightTo(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen, CompanyRightsEnum.SeeWallet) == false)
                return RedirectToHomeWithError("You cannot do that!");

            var walletID = company.Entity.WalletID;
            var money = walletRepository.GetMoney(walletID).ToList();

            var vm = new CompanyWalletViewModel(company, money);

            return View(vm);

        }
        private void prepareCreateCompanyViewModel(CreateCompanyViewModel vm)
        {
            var region = SessionHelper.CurrentEntity.GetCurrentRegion();
            var country = region.Country;
            var entity = SessionHelper.CurrentEntity;

            vm.CountryName = country.Entity.Name;
            vm.CountryID = country.ID;
            vm.RegionName = region.Name;
            vm.RegionID = region.ID;
            vm.CountryFee = new MoneyViewModel(country.Currency, country.CountryPolicy.GetCompanyCost(entity));
            vm.AdminFee = new MoneyViewModel(currencyRepository.Gold, configurationRepository.First().GetCompanyFee(entity));

            vm.LoadSelectList(companyRepository, companyService);
        }

        private void prepareCreateCompanyViewModel(CreateCompanyAsOrganisationViewModel vm)
        {
            var region = regionRepository.GetById(vm.RegionID);
            var entity = SessionHelper.CurrentEntity;
            var country = entity.Organisation.Country;

            if (region != null)
                vm.RegionName = region.Name;

            vm.CountryName = country.Entity.Name;
            vm.CountryID = country.ID;
            vm.CountryFee = new MoneyViewModel(country.Currency, country.CountryPolicy.GetCompanyCost(entity));
            vm.AdminFee = new MoneyViewModel(currencyRepository.Gold, configurationRepository.First().GetCompanyFee(entity));

            vm.LoadSelectList(companyRepository, companyService);
            vm.LoadSelectList(regionRepository);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        public ActionResult ContractDetails(int contractID)
        {
            var contract = jobOfferRepository
                .GetById(contractID)
                .ContractJobOffer;

            var vm = new ContractDetailsViewModel(contract, contract.JobOffer.ID);


            return PartialView(vm);
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult AcceptContract(int contractID)
        {
            var citizen = SessionHelper.CurrentEntity?.Citizen;
            var result = companyService.CanStartWorkAt(contractID, citizen);
            if (result.IsError)
                return JsonError(result);

            var jobOffer = jobOfferRepository.GetById(contractID);

            EmployCitizenParameters pars = new EmployCitizenParameters()
            {
                CitizenID = citizen.ID,
                CompanyID = jobOffer.CompanyID,
                JobOfferID = contractID,
                Salary = jobOffer.GetStartingSalary(),
                ContractOffer = jobOffer.ContractJobOffer
            };

            var company = jobOffer.Company;
            var employee = companyService.EmployCitizen(pars);
            companyService.InformAboutNewEmployee(company, employee);

            AddInfo("You are working at {0} from now on.", employee.Company.Entity.Name);
            return JsonRedirect("View", new { companyID = jobOffer.CompanyID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [IsCitizen]
        [HttpPost]
        [Route("Company/Create")]
        public ActionResult Create(CreateCompanyViewModel vm)
        {
            if (!canCreateCompany())
                return RedirectToAction("Index", "Home");

            prepareCreateCompanyViewModel(vm);

            CompanyValidator validator = new CompanyValidator(ModelState);
            validator.Validate(vm, SessionHelper.CurrentEntity);

            if (validator.IsValid)
            {
                var entity = SessionHelper.CurrentEntity;
                var region = entity.GetCurrentRegion();
                var country = region.Country;
                vm.CompanyName = vm.CompanyName.Trim();

                var company = companyService.CreateCompany(vm.CompanyName.Trim(), (ProductTypeEnum)vm.ProducedProductID.Value, region.ID, entity.EntityID);

                Transaction adminTransaction, countryTransaction;
                prepareCreateCompanyTransactions(vm, entity, company, country, out adminTransaction, out countryTransaction);

                transactionService.MakeTransaction(adminTransaction);
                transactionService.MakeTransaction(countryTransaction);

                return RedirectToAction("Index", "Business");
            }
            AddError(validator);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [IsOrganisation]
        [HttpPost]
        [Route("Company/Create")]
        public ActionResult CreateAsOrganisation(CreateCompanyAsOrganisationViewModel vm)
        {
            if (!canCreateCompany())
                return RedirectToAction("Index", "Home");

            prepareCreateCompanyViewModel(vm);

            CompanyValidator validator = new CompanyValidator(ModelState);
            validator.Validate(vm, SessionHelper.CurrentEntity);

            if (validator.IsValid)
            {
                var entity = SessionHelper.CurrentEntity;
                var region = regionRepository.GetById(vm.RegionID);
                var country = region.Country;
                vm.CompanyName = vm.CompanyName.Trim();

                var company = companyService.CreateCompany(vm.CompanyName, (ProductTypeEnum)vm.ProducedProductID.Value, region.ID, entity.EntityID);

                Transaction adminTransaction, countryTransaction;
                prepareCreateCompanyTransactions(vm, entity, company, country, out adminTransaction, out countryTransaction);

                transactionService.MakeTransaction(adminTransaction);
                transactionService.MakeTransaction(countryTransaction);

                return RedirectToAction("Index", "Business");
            }

            return View(vm);
        }

        private void prepareCreateCompanyTransactions(CreateCompanyViewModel vm, Entity entity, Company company, Country country, out Transaction adminTransaction, out Transaction countryTransaction)
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
                Arg2 = string.Format("{0}({1}) created company {2}({3})", entity.Name, entity.EntityID, company.Entity.Name, company.ID),
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

        private void prepareMarketOfferTransactions(MoneyViewModel vm, Entity entity, Company company, out Transaction countryTransaction)
        {

            var countryCurrency = Persistent.Currencies.First(c => c.ID == vm.CurrencyID);
            var country = Persistent.Countries.First(c => c.CurrencyID == vm.CurrencyID);

            var countryMoney = new Money()
            {
                Amount = vm.Quantity,
                Currency = countryCurrency
            };

            countryTransaction = new Transaction()
            {
                Arg1 = "Market offer creation - market offer cost",
                Arg2 = string.Format("{0}({1}) created market offer of {2}({3})", entity.Name, entity.EntityID, company.Entity.Name, company.ID),
                DestinationEntityID = country.ID,
                Money = countryMoney,
                SourceEntityID = entity.EntityID,
                TransactionType = TransactionTypeEnum.MarketOfferCost
            };

            
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CreateJobOffer(int companyID)
        {
            Company company = companyRepository.GetById(companyID);
            if (company == null)
            {
                return RedirectCompanyDoesNotExists();
            }

            CreateJobOfferViewModel vm = new CreateJobOfferViewModel()
            {
                CompanyID = companyID
            };
            return View(vm);
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult CreateJobOffer(CreateJobOfferViewModel vm)
        {
            switch (vm.JobType)
            {
                case JobOfferTypeEnum.Normal:
                    return RedirectToAction("CreateNormalJobOffer", new { companyID = vm.CompanyID });
                case JobOfferTypeEnum.Contract:
                    return RedirectToAction("CreateContractJobOffer", new { companyID = vm.CompanyID });
                default:
                    return View(vm);
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CreateContractJobOffer(int companyID)
        {
            Company company = companyRepository.GetById(companyID);
            if (company == null)
            {
                return RedirectCompanyDoesNotExists();
            }

            if (!haveRights(CompanyRightsEnum.PostJobOffers, company))
                return NoAccessRights(companyID);

            var entity = SessionHelper.CurrentEntity;
            var country = entity.GetCurrentRegion().Country;
            var countryPolicy = country.CountryPolicy;

            MoneyViewModel countryFeeVM = new MoneyViewModel(country.Currency, countryPolicy.ContractJobMarketFee);

            CreateContractJobOfferViewModel vm = new CreateContractJobOfferViewModel()
            {
                CompanyID = companyID,
                JobMarketFee = countryFeeVM
            };


            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/MarketOffers/{PagingParam.PageNumber:int=1}")]
        public ActionResult MarketOffers(int companyID, PagingParam pagingParam)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var company = companyRepository.GetById(companyID);
            var offers = marketOfferRepository.GetMarketOfferModel().Where(o => o.Company.EntityID == companyID);

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            var vm = new CompanyMarketOfferListViewModel(company, offers, pagingParam, rights, marketService);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult CreateContractJobOffer(CreateContractJobOfferViewModel vm)
        {
            if (!haveRights(CompanyRightsEnum.PostJobOffers, vm.CompanyID))
                return NoAccessRights(vm.CompanyID);


            var entity = SessionHelper.CurrentEntity;
            var citizen = SessionHelper.LoggedCitizen;
            var country = entity.GetCurrentRegion().Country;
            var countryPolicy = country.CountryPolicy;
            var company = companyRepository.GetById(vm.CompanyID);

            MoneyViewModel countryFeeVM = new MoneyViewModel(country.Currency, countryPolicy.NormalJobMarketFee);

            vm.JobMarketFee = countryFeeVM;

            CompanyValidator validator = new CompanyValidator(ModelState);
            validator.Validate(vm, SessionHelper.CurrentEntity);

            if (validator.IsValid)
            {
                CreateContractJobOfferParams pars = new CreateContractJobOfferParams()
                {
                    Amount = vm.Amount,
                    CompanyID = vm.CompanyID,
                    MinimumSkill = vm.MinSkill.Value,
                    MinimumHP = vm.MinHP,
                    Length = vm.Length,
                    MinimumSalary = vm.MinimumSalary.Value,
                    SigneeID = SessionHelper.LoggedCitizen.ID,
                    Cost = vm.JobMarketFee.Quantity,
                    CurrencyID = vm.JobMarketFee.CurrencyID

                };

                if (vm.PostOfferOnJobMarket)
                    pars.CountryID = company.Region.CountryID;
                else
                    pars.Cost = 0;

                jobOfferService.CreateJobOffer(pars);
                return RedirectToAction("View", new { companyID = vm.CompanyID });
            }

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CreateNormalJobOffer(int companyID)
        {
            Company company = companyRepository.GetById(companyID);
            if (company == null)
            {
                return RedirectCompanyDoesNotExists();
            }

            if (!haveRights(CompanyRightsEnum.PostJobOffers, company))
                return NoAccessRights(companyID);

            var entity = company.Entity;
            var country = entity.GetCurrentRegion().Country;
            var countryPolicy = country.CountryPolicy;

            MoneyViewModel countryFeeVM = new MoneyViewModel(country.Currency, countryPolicy.NormalJobMarketFee);

            CreateNormalJobOfferViewModel vm = new CreateNormalJobOfferViewModel()
            {
                CompanyID = companyID,
                JobMarketFee = countryFeeVM
            };


            vm.Info = new CompanyInfoViewModel(company);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult CreateNormalJobOffer(CreateNormalJobOfferViewModel vm)
        {
            if (!haveRights(CompanyRightsEnum.PostJobOffers, vm.CompanyID))
                return NoAccessRights(vm.CompanyID);

            var company = companyRepository.GetById(vm.CompanyID);

            var entity = company.Entity;
            var country = entity.GetCurrentRegion().Country;
            var countryPolicy = country.CountryPolicy;

            MoneyViewModel countryFeeVM = new MoneyViewModel(country.Currency, countryPolicy.NormalJobMarketFee);

            vm.JobMarketFee = countryFeeVM;

            CompanyValidator validator = new CompanyValidator(ModelState);

            validator.Validate(vm, SessionHelper.CurrentEntity);

            if (validator.IsValid)
            {
                CreateNormalJobOfferParams pars = new CreateNormalJobOfferParams()
                {
                    Amount = vm.Amount,
                    CompanyID = vm.CompanyID,
                    MinimumSkill = vm.MinSkill.Value,
                    Salary = vm.Salary.Value,
                    Cost = vm.JobMarketFee.Quantity,
                    CurrencyID = vm.JobMarketFee.CurrencyID,

                };

                if (vm.PostOfferOnJobMarket)
                    pars.CountryID = company.Region.CountryID;
                else
                    pars.Cost = 0;


                jobOfferService.CreateJobOffer(pars);
                return RedirectToAction("View", new { companyID = vm.CompanyID });
            }
            vm.Info = new CompanyInfoViewModel(company);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        [Route("JobOffer/{jobOfferID:int}/Accept")]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobOfferID">JobOfferID</param>
        /// <returns></returns>
        public ActionResult AcceptJobOffer(int jobOfferID)
        {

            var citizen = SessionHelper.CurrentEntity.Citizen;

            var result = companyService.CanStartWorkAt(jobOfferID, citizen);

            if (result.IsError)
            {
                AddError(result);
                return RedirectBack();
            }


            var jobOffer = jobOfferRepository.GetById(jobOfferID);

            EmployCitizenParameters pars = new EmployCitizenParameters()
            {
                CitizenID = citizen.ID,
                CompanyID = jobOffer.CompanyID,
                JobOfferID = jobOfferID,
                Salary = jobOffer.GetStartingSalary(),
                ContractOffer = jobOffer.ContractJobOffer
            };

            var company = jobOffer.Company;
            var employee = companyService.EmployCitizen(pars);
            companyService.InformAboutNewEmployee(company, employee);
            return RedirectToAction("View", new { companyID = jobOffer.CompanyID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult LeaveJob(int companyID)
        {
            Company company = companyRepository.GetById(companyID);
            if (company == null)
            {
                return RedirectCompanyDoesNotExists();
            }
            var citizen = SessionHelper.CurrentEntity.Citizen;

            if (!companyService.IsWorkingAtCompany(citizen.ID, companyID))
            {
                AddMessage(new PopupMessageViewModel()
                {
                    Content = "You are not working here!",
                    MessageType = PopupMessageType.Error
                });

                return RedirectToAction("View", new { companyID = companyID });
            }

            var employee = citizen.CompanyEmployee;

            if (companyService.CanLeaveJob(employee))
            {
                companyService.LeaveJob(employee.CitizenID, employee.CompanyID);
            }

            AddMessage(new PopupMessageViewModel()
            {
                Content = "You left the job.",
                MessageType = PopupMessageType.Info
            });

            return RedirectToAction("View", new { companyID = companyID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Work(int ID)
        {
            Company company = companyRepository.GetById(ID);
            var citizen = SessionHelper.CurrentEntity.Citizen;

            MethodResult result = companyService.CanWork(citizen, company);

            if (result.IsError)
                return RedirectBackWithError(result);

            companyService.Work(citizen.ID, ID);
            var stats = companyService.GetWorkStatistics(citizen.ID);

            AddInfo(string.Format("You worked by creating {0:0.00} {1} and earned {2} {3}",
                stats.Production, company.Product.Name, stats.Salary.Amount, stats.Salary.Currency.Symbol));

            companyRepository.ReloadEntity(company.Entity.Equipment);

            return RedirectToAction("View", new { companyID = ID });
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/JobOffers")]
        public ActionResult JobOffers(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            if (company == null)
            {
                return RedirectCompanyDoesNotExists();
            }

            CompanyJobOffersView vm = new CompanyJobOffersView(company);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/Inventory")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult Inventory(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            if (company == null)
                return RedirectToHomeWithError("Company does not exist!");

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);
            if (rights.CanManageEquipment == false)
                return RedirectToHomeWithError("You cannot see this inventory!");

            var vm = new CompanyEquipmentViewModel(company, company.Entity.Equipment);

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult TopBar(int ID)
        {
            CompanyTobBarViewModel vm = new CompanyTobBarViewModel();
            var company = companyRepository.GetById(ID);

            vm.CountryName = company.Region.Country.Entity.Name;
            vm.RegionName = company.Region.Name;
            vm.Avatar = new ImageViewModel(company.Entity.ImgUrl);
            vm.CompanyID = ID;
            vm.Name = company.Entity.Name;
            vm.IsWorkingHere = companyService.IsWorkingAtCompany(SessionHelper.LoggedCitizen.ID, ID);

            return PartialView(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}")]
        public ActionResult View(int companyID)
        {
            var company = companyRepository
                .GetById(companyID);

            if (company == null)
            {
                return RedirectCompanyDoesNotExists();
            }

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            ViewCompanyViewModel vm = CompanyViewModelChooser.GetViewModel(company, productRepository, companyService, rights, regionRepository, regionService, hospitalRepository);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/CreateMarketOffer")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult CreateMarketOffer(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            if (company == null)
                return RedirectCompanyDoesNotExists();

            if (!haveRights(CompanyRightsEnum.PostMarketOffers, company))
                return NoAccessRights(companyID);

            List<int> embargoesCountries = getEmbargoedCountries(company);

            CompanyAddMarketOfferViewModel vm = null;
            switch ((CompanyTypeEnum)company.CompanyTypeID)
            {
                case CompanyTypeEnum.Manufacturer:
                case CompanyTypeEnum.Producer:
                case CompanyTypeEnum.Construction:
                    if (company.GetCompanyType() == CompanyTypeEnum.Construction && company.GetProducedProductType() != ProductTypeEnum.House)
                        break;
                    vm = new CompanyAddSpecificMarketOfferViewModel(company, embargoesCountries, (ProductTypeEnum)company.ProductID, company.Quality);
                    break;
                default:
                    vm = new CompanyAddMarketOfferViewModel(company, embargoesCountries);
                    break;
            }


            return View(vm);
        }

        private List<int> getEmbargoedCountries(Company company)
        {
            List<int> embargoesCountries = new List<int>();
            if (company.Region.CountryID.HasValue)
            {

                embargoesCountries.AddRange(
                    countryRepository.GetEmbargoes(company.Region.CountryID.Value)
                    .Select(c => c.ID)
                    .ToList()
                    );
            }

            return embargoesCountries;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Employee/{citizenID:int}/Manage")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult ManageEmployee(int citizenID)
        {
            var employee = companyEmployeeRepository.GetById(citizenID);
            if (employee == null)
                return RedirectToHomeWithError("Employee does not exist!");

            var company = employee.Company;

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);


            var isEmployeeManager = companyRepository.IsManager(company.ID, employee.CitizenID);

            if (rights.CanManageWorkers == false && rights.CanManageManagers == false)
                return RedirectToHomeWithError("You cannot manage this employee!");

            ManageEmployeeViewModel vm = null;
            if (employee.JobContractID == null)
                vm = new ManageEmployeeViewModel(employee, rights, isEmployeeManager);
            else
                vm = new ManageContractEmployeeViewModel(employee, rights, isEmployeeManager);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Employee/{citizenID:int}/Manage")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult ManageEmployee(int citizenID, ManageEmployeeViewModel vm)
        {
            var employee = companyEmployeeRepository.GetById(citizenID);
            if (employee == null)
                return RedirectToHomeWithError("Employee does not exist!");

            var company = employee.Company;

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (rights.CanManageWorkers == false && rights.CanManageManagers == false)
                return RedirectToHomeWithError("You cannot manage this employee!");

            if (employee.JobContract != null)
                return RedirectToHomeWithError("I see what you wanted to do here :)");

            if (ModelState.IsValid)
            {
                ChangeWorkingConditions(vm, employee);

                return RedirectToAction("View", new { companyID = company.ID });
            }

            var isEmployeeManager = companyRepository.IsManager(company.ID, employee.CitizenID);
            return View("ManageEmployee", new ManageEmployeeViewModel(employee, rights, isEmployeeManager));
        }

        private void ChangeWorkingConditions(ManageEmployeeViewModel vm, CompanyEmployee employee)
        {
            bool salaryChanged = Math.Abs(employee.Salary - (decimal)vm.Salary) < 0.0001m;
            bool minHpChanged = employee.MinHP != vm.MinimumHP;

            employee.Salary = (decimal)vm.Salary;
            employee.MinHP = vm.MinimumHP;
            companyEmployeeRepository.SaveChanges();
            if (salaryChanged)
                employeeService.InformAboutSalaryChange(employee);
            if (minHpChanged)
                employeeService.InformAboutMinimumHPChange(employee);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Employee/{citizenID:int}/Manage-c")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult ManageContractEmployee(int citizenID, ManageEmployeeViewModel vm)
        {
            var employee = companyEmployeeRepository.GetById(citizenID);
            if (employee == null)
                return RedirectToHomeWithError("Employee does not exist!");

            var company = employee.Company;

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (rights.CanManageWorkers == false && rights.CanManageManagers == false)
                return RedirectToHomeWithError("You cannot manage this employee!");

            if (employee.JobContractID == null)
                return RedirectToHomeWithError("I see what you wanted to do here :)");

            var validator = new ManageEmployeeViewModelValidator(ModelState, companyEmployeeRepository);

            if (validator.Validate(vm))
            {
                ChangeWorkingConditions(vm, employee);

                return RedirectToAction("View", new { companyID = company.ID });
            }

            var isEmployeeManager = companyRepository.IsManager(company.ID, employee.CitizenID);
            return View("ManageEmployee", new ManageContractEmployeeViewModel(employee, rights, isEmployeeManager));
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult KickEmployee(int citizenID)
        {
            var employee = companyEmployeeRepository.GetById(citizenID);
            if (employee == null)
                return RedirectToHomeWithError("Employee does not exist!");

            var company = employee.Company;

            var rights = companyService.GetCompanyRights(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (rights.CanManageWorkers == false && rights.CanManageManagers == false)
                return RedirectToHomeWithError("You cannot manage this employee!");

            if (employee.JobContractID != null)
            {
                var contract = employee.JobContract;

                if (contract.Length >= 0 && contract.AbusedByEmployee == false)
                {
                    AddError("You cannot fire contracted worker which has not abused contract!");
                    return RedirectToAction("View", new { companyID = company.ID });
                }
            }

            AddInfo("{0} was fired!", employee.Citizen.Entity.Name);
            companyEmployeeRepository.Remove(employee);
            companyEmployeeRepository.SaveChanges();
            employeeService.InformAboutDismiss(employee, company);

            return RedirectToAction("View", new { companyID = company.ID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Company/{companyID:int}/CreateMarketOffer")]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation, EntityTypeEnum.Company)]
        public ActionResult CreateMarketOffer(int companyID, CompanyAddMarketOfferViewModel vm)
        {
            var company = companyRepository.GetById(companyID);
            if (company == null)
                return RedirectCompanyDoesNotExists();

            switch ((CompanyTypeEnum)company.CompanyTypeID)
            {
                case CompanyTypeEnum.Manufacturer:
                case CompanyTypeEnum.Producer:
                    vm.ProductID = company.ProductID;
                    break;
                case CompanyTypeEnum.Shop:
                    //shop does not have specific product id
                    break;
            }

            if (!haveRights(CompanyRightsEnum.PostMarketOffers, company))
                return NoAccessRights(companyID);

            List<int> embargoesCountries = getEmbargoedCountries(company);

            CompanyAddMarketOfferViewModel oldvm = vm;
            switch ((CompanyTypeEnum)company.CompanyTypeID)
            {
                case CompanyTypeEnum.Manufacturer:
                case CompanyTypeEnum.Producer:
                    vm = new CompanyAddSpecificMarketOfferViewModel(company, embargoesCountries, (ProductTypeEnum)company.ProductID, company.Quality);
                    break;
                default:
                    vm = new CompanyAddMarketOfferViewModel(company, embargoesCountries);
                    break;
            }

            vm.Amount = oldvm.Amount;
            vm.Quality = oldvm.Quality;
            vm.ProductID = oldvm.ProductID;
            vm.Price = oldvm.Price;
            vm.CountryID = oldvm.CountryID;

            CompanyAddMarketOfferValidator validator = new CompanyAddMarketOfferValidator(ModelState, countryRepository);

            if (validator.Validate(vm))
            {

                MethodResult result;
                if ((result = marketService.CanMakeOffer(company, vm.Amount.Value, vm.Price.Value, (ProductTypeEnum)vm.ProductID, vm.Quality, vm.CountryID, embargoesCountries)).isSuccess == false)
                    return RedirectBackWithError(result);


                if (vm.CountryID == 0)
                    vm.CountryID = null;


                AddMarketOfferParameters ps = new AddMarketOfferParameters()
                {
                    Amount = vm.Amount.Value,
                    CompanyID = companyID,
                    CountryID = vm.CountryID,
                    Price = vm.Price.Value, //has been validated - always not null
                    ProductType = (ProductTypeEnum)vm.ProductID,
                    Quality = vm.Quality
                };

                using (var trs = transactionScopeProvider.CreateTransactionScope())
                {
                    marketService.AddOffer(ps);

                    if (vm.CountryID > 0)
                    {
                        Transaction countryTransaction;

                        var currency = Persistent.Countries.First(c => c.ID == vm.CountryID).Currency;
                        var countryPolicy = countryRepository.GetCountryPolicyById(vm.CountryID.Value);

                        prepareMarketOfferTransactions(new MoneyViewModel(currency, countryPolicy.MarketOfferCost), SessionHelper.CurrentEntity, company, out countryTransaction);

                        companyFinanceSummaryService.AddFinances(company, new MarketOfferCostFinance(countryTransaction.Money.Amount, countryTransaction.Money.Currency.ID));

                        transactionService.MakeTransaction(countryTransaction);
                    }

                    trs.Complete();
                }

                return RedirectToAction("MarketOffers", new { companyID = companyID });
            }
            else
            {
                return View(vm);
            }

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/MarketOfferPrice")]
        public ActionResult MarketOfferPrice(int countryID)
        {
            var countryPolicy = countryRepository.GetCountryPolicyById(countryID);

            var currency = Persistent.Countries.First(c => c.ID == countryID).Currency;

            var vm = new MoneyViewModel(currency, countryPolicy.MarketOfferCost);

            return PartialView(vm);
        }

        private ActionResult RedirectCompanyDoesNotExists()
        {
            PopupMessageViewModel popup = new PopupMessageViewModel()
            {
                Content = "Company does not exists",
                MessageType = PopupMessageType.Error
            };
            AddMessage(popup);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NoAccessRights(int companyID)
        {
            var vm = new NoAccessRightsViewModel()
            {
                CompanyID = companyID
            };

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Organisation)]
        public ActionResult Index()
        {
            List<ManageableCompanyInfo> companies = companyService.GetManageableCompanies(SessionHelper.CurrentEntity);
            CompanyListViewModel vm = new CompanyListViewModel(companies);
            return View(vm);
        }

        private bool canCreateCompany()
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity == null)
                return false;

            return entity.EntityTypeID == (int)EntityTypeEnum.Citizen
                || entity.EntityTypeID == (int)EntityTypeEnum.Organisation
                || entity.EntityTypeID == (int)EntityTypeEnum.Country;
        }

        private bool haveRights(CompanyRightsEnum rights, int companyID)
        {
            var company = companyRepository.GetById(companyID);

            return haveRights(rights, company);
        }

        private bool haveRights(CompanyRightsEnum right, Company company)
        {
            var entity = SessionHelper.CurrentEntity;

            if (company.ID == entity.EntityID)
                return true;

            if (entity.EntityID == company.OwnerID)
                return true;

            return companyService.DoesHaveRightTo(company, entity, SessionHelper.LoggedCitizen, right);
        }

        public PartialViewResult GetProductCost(int companyID, int productID, int? countryID, int amount, decimal price)
        {
            if (countryID == 0)
                countryID = null;

            var company = companyRepository.GetById(companyID);

            var cost = marketService.CalculateProductCost(amount, price, company.Region.CountryID, countryID, (ProductTypeEnum)productID);

            var vm = new ProductCostViewModel(cost, company.Region.CountryID, countryID, amount);

            return PartialView(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/Managers")]
        public ActionResult Managers(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            var entity = SessionHelper.CurrentEntity;

            var result = companyService.CanSeeManagers(company, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            var currentRights = companyService.GetCompanyRights(company, entity, SessionHelper.LoggedCitizen);

            var vm = new CompanyManagersViewModel(company, currentRights);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult AddManager(int companyID, int entityID)
        {
            var company = companyRepository.GetById(companyID);
            var entity = entityRepository.GetById(entityID);

            var rights = new CompanyRights(false);
            rights.IsManager = true;

            MethodResult result = companyService.CanAddManager(company, SessionHelper.CurrentEntity, entity, rights);
            if (result.IsError)
                return RedirectBackWithError(result);

            companyService.AddManager(company, entity.Citizen, rights);
            AddSuccess($"Added {entity.Name} as manager");
            return RedirectToAction("Managers", new { companyID = companyID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ChangeManager(int managerID, int priority, int[] rights)
        {
            try
            {
                var manager = companyManagerRepository.GetById(managerID);
                var entity = SessionHelper.CurrentEntity;
                var managerRights = companyService.GetCompanyRights(manager.Company, entity, SessionHelper.LoggedCitizen);
                foreach (CompanyRightsEnum right in Enum.GetValues(typeof(CompanyRightsEnum)).Cast<CompanyRightsEnum>())
                {
                    if (rights?.Contains((int)right) ?? false)
                        managerRights[right] = true;
                    else
                        managerRights[right] = false;
                }

                managerRights.Priority = priority;

                MethodResult result = companyService.CanModifyManager(manager.Company, entity, manager, managerRights);
                if (result.IsError)
                    return JsonError(result);

                companyService.ModifyManager(manager, managerRights);
                return JsonSuccess("Manager rights has been changed.");
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult RemoveManager(int managerID)
        {
            var manager = companyManagerRepository.GetById(managerID);
            var entity = SessionHelper.CurrentEntity;

            var result = companyService.CanRemoveManager(manager, entity);
            if (result.IsError)
                return RedirectBackWithError(result);

            companyService.RemoveManager(manager);
            AddSuccess("Manager has been removed!");
            return RedirectToAction("Managers", "Company", new { companyID = manager.CompanyID });
        }


        public JsonResult GetAppropriateManagers(Select2Request request)
        {
            string search = request.Query.Trim().ToLower();

            var query = citizenRepository
                .Where(citizen => citizen.Entity.Name.ToLower().Contains(search))
                .OrderBy(citizen => citizen.Entity.Name)
                .Select(citizen => new Select2Item()
                {
                    id = citizen.ID,
                    text = citizen.Entity.Name
                });

            return Select2Response(query, request);
        }

        [Route("Company/{companyID:int}/Management")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Management(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            if (company == null)
                return RedirectBackWithError("Company does not exist!");

            var vm = new CompanyManagementViewModel(company);

            return View(vm);
        }


    }
}