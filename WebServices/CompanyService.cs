using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using WebServices.structs;
using WebServices.BigParams.Company;
using Entities.Extensions;
using Common;
using WebServices.Helpers;
using Common.Exceptions;
using Common.Operations;
using Common.Language;
using System.Transactions;
using Entities.Groups;
using Weber.Html;
using WebServices.structs.Companies;
using WebServices.PathFinding;
using WebServices.Calculators.ProductionPoints;
using Common.Transactions;
using System.Web.Mvc;
using Entities.enums.Attributes;
using WebServices.Companies;

namespace WebServices
{
    public class CompanyService : BaseService, ICompanyService
    {
        ICompanyRepository companyRepository;
        IEntityService entityService;
        ICompanyManagerRepository companyManagerRepository;
        IJobOfferService jobOfferService;
        ICompanyEmployeeRepository companyEmployeeRepository;
        ICitizenRepository citizenRepository;
        ITransactionsService transactionService;
        IEquipmentRepository equipmentRepository;
        IEquipmentService equipmentService;
        IProductService productService;
        IProductRepository productRepository;
        ICitizenService citizenService;
        IConfigurationRepository configurationRepository;
        IRegionRepository regionRepository;
        IWarningService warningService;
        IJobOfferRepository jobOfferRepository;
        IContractService contractService;
        IWalletService walletService;
        IPopupService popupService;
        IRegionService regionService;
        ICompanyFinanceSummaryService companyFinanceSummaryService;

        public CompanyService(ICitizenService citizenService, IConfigurationRepository configurationRepository,
            IEquipmentRepository equipmentRepository, IProductService productService, IProductRepository productRepository,
            ICitizenRepository citizenRepository, ITransactionsService transactionService,
            IJobOfferService jobOfferService, ICompanyEmployeeRepository companyEmployeeRepository,
            ICompanyRepository companyRepository, IEntityService entityService, ICompanyManagerRepository companyManagerRepository,
            IRegionRepository regionRepository, IWarningService warningService, IJobOfferRepository jobOfferRepository,
            IEquipmentService equipmentService, IContractService contractService, IWalletService walletService, IPopupService popupService,
            IRegionService regionService, ICompanyFinanceSummaryService companyFinanceSummaryService)
        {
            this.companyRepository = companyRepository;
            this.entityService = entityService;
            this.companyManagerRepository = companyManagerRepository;
            this.jobOfferService = jobOfferService;
            this.companyEmployeeRepository = companyEmployeeRepository;
            this.citizenRepository = citizenRepository;
            this.transactionService = transactionService;
            this.equipmentRepository = equipmentRepository;
            this.productService = productService;
            this.productRepository = productRepository;
            this.citizenService = Attach(citizenService);
            this.configurationRepository = configurationRepository;
            this.regionRepository = regionRepository;
            this.warningService = Attach(warningService);
            this.jobOfferRepository = jobOfferRepository;
            this.equipmentService = Attach(equipmentService);
            this.contractService = Attach(contractService);
            this.walletService = Attach(walletService);
            this.popupService = Attach(popupService);
            this.regionService = Attach(regionService);
            this.companyFinanceSummaryService = Attach(companyFinanceSummaryService);

        }

        public List<ManageableCompanyInfo> GetManageableCompanies(Entity entity)
        {
            return companyRepository.Where(c => c.OwnerID == entity.EntityID
            || c.CompanyManagers.Any(man => man.EntityID == entity.EntityID))
            .Select(c => new ManageableCompanyInfo()
            {
                ID = c.ID,
                ImgUrl = c.Entity.ImgUrl,
                Name = c.Entity.Name,
                ProductID = c.ProductID,
                UnreadMessages = c.Entity.MailboxMessages.Count(msg => msg.Unread),
                UnreadWarnings = c.Entity.Warnings.Count(w => w.Unread)
            }).ToList();
        }



        public IEnumerable<Money> GetCompanyCreationCost(Region region, EntityTypeEnum creator)
        {
            var country = region.Country;
            var policy = country.CountryPolicy;

            var countryCreationFee = new Money()
            {
                Currency = Persistent.Currencies.GetById(country.CurrencyID)
            };

            if (creator == EntityTypeEnum.Citizen)
                countryCreationFee.Amount = policy.CitizenCompanyCost;
            else if (creator == EntityTypeEnum.Organisation)
                countryCreationFee.Amount = policy.OrganisationCompanyCost;
            if (creator == EntityTypeEnum.Citizen || creator == EntityTypeEnum.Organisation)
                yield return countryCreationFee;

            decimal adminGoldCost = 0m;

            switch (creator)
            {
                case EntityTypeEnum.Citizen:
                    adminGoldCost = ConfigurationHelper.Configuration.CompanyCitizenFee; break;
                case EntityTypeEnum.Organisation:
                    adminGoldCost = ConfigurationHelper.Configuration.CompanyOrganisationFee; break;
                case EntityTypeEnum.Country:
                    adminGoldCost = ConfigurationHelper.Configuration.CompanyCountryFee; break;
            }

            yield return new Money()
            {
                Currency = Persistent.Currencies.Gold,
                Amount = adminGoldCost
            };
        }

        public IEnumerable<CompanyTypeEnum> GetCompaniesCreatableByPlayers()
        {
            CompanyTypeEnum[] excluded =
                {
                CompanyTypeEnum.Developmenter
            };


            foreach (var type in Enum.GetValues(typeof(CompanyTypeEnum)).Cast<CompanyTypeEnum>())
                if (excluded.Contains(type) == false)
                    yield return type;
        }

        public IEnumerable<ProductTypeEnum> GetCompaniesProductsCreatableByPlayers()
        {
            ProductTypeEnum[] excluded =
                {
                ProductTypeEnum.DefenseSystem,
                ProductTypeEnum.Hospital,
                ProductTypeEnum.Development
            };


            foreach (var type in Enum.GetValues(typeof(ProductTypeEnum)).Cast<ProductTypeEnum>())
                if (excluded.Contains(type) == false)
                    yield return type;
        }

        public void AddManager(Company company, Citizen citizen, CompanyRights rights)
        {
            CompanyManager manager = new CompanyManager()
            {
                EntityID = citizen.ID,
                CompanyID = company.ID,
            };

            var companyLink = EntityLinkCreator.Create(company.Entity);

            var msg = $"From this day on you are new manager of {companyLink}";
            using (NoSaveChanges)
                warningService.AddWarning(citizen.ID, msg);

            rights.FillEntity(ref manager);
            companyManagerRepository.Add(manager);
            ConditionalSaveChanges(companyManagerRepository);
        }

        public MethodResult CanStartWorkAt(int jobOfferID, Citizen citizen)
        {

            var jobOffer = jobOfferRepository.GetById(jobOfferID);
            if (jobOffer == null)
            {
                return new MethodResult("Job offer does not exist!");
            }

            if (citizen == null)
                return new MethodResult("Only citizens can work!");

            if (citizen.CompanyEmployee != null)
            {
                return new MethodResult("You are working in another company!");
            }

            return MethodResult.Success;
        }

        public Company CreateCompanyForCountry(string name, ProductTypeEnum productType, int regionID, Country country, bool payForCreation = true)
        {
            var company = CreateCompany(name, productType, regionID, country.ID);
            if (payForCreation)
                CountryPayForCompanyCreate(country, company);
            return company;
        }

        public Company CreateCompany(string name, ProductTypeEnum productType, int regionID, int ownerID)
        {
            using (var scope = transactionScopeProvider.CreateTransactionScope())
            {
                Entity entity = entityService.CreateEntity(name, EntityTypeEnum.Company);
                entity.Equipment.ItemCapacity = 5000;
                var company = new Company()
                {
                    ID = entity.EntityID,
                    OwnerID = ownerID,
                    ProductID = (int)productType,
                    Quality = 1,
                    RegionID = regionID,
                    WorkTypeID = (int)getWorkTypeForProduct(productType),
                    CompanyTypeID = (int)getCompanyTypeForProduct(productType)
                };

                switch (productType)
                {
                    case ProductTypeEnum.MedicalSupplies:
                        createHospitalForCompany(company);
                        break;
                }

                if (productType.HasEnumAttribute<ConstructionAttribute>())
                    createConstructionForCompany(company);

                companyRepository.Add(company);
                companyRepository.SaveChanges();

                var equipment = entity;

                equipmentService.GiveItem(ProductTypeEnum.Fuel, 50, 1, entity.Equipment);
                if (productType == ProductTypeEnum.SellingPower)
                    equipmentService.GiveItem(ProductTypeEnum.SellingPower, 10, 1, entity.Equipment);

                scope?.Complete();

                return company;
            }
        }

        private void createHospitalForCompany(Company company)
        {
            var hospital = new Hospital()
            {
                CompanyID = company.ID,
                HealingEnabled = false,
                HealingPrice = null
            };

            company.Hospital = hospital;
        }

        private void createConstructionForCompany(Company company)
        {
            company.Construction = new Construction()
            {
                Progress = 0
            };
        }

        public void CountryPayForCompanyCreate(Country country, Company company)
        {
            var adminFee = new Money()
            {
                Currency = GameHelper.Gold,
                Amount = ConfigurationHelper.Configuration.CompanyCountryFee
            };



            var adminTransaction = new structs.Transaction()
            {
                Arg1 = "Admin Fee",
                Arg2 = string.Format("{0}({1}) created company {2}({3})", country.Entity.Name, country.Entity.EntityID, company.Entity.Name, company.ID),
                DestinationEntityID = null,
                Money = adminFee,
                SourceEntityID = country.ID,
                TransactionType = TransactionTypeEnum.CompanyCreate
            };

            transactionService.MakeTransaction(adminTransaction);
        }

        public void ProcessDayChange(int newDay)
        {
            var shops = companyRepository
                .Where(c => c.CompanyTypeID == (int)CompanyTypeEnum.Shop)
                .ToList();

            foreach (var shop in shops)
            {
                var item = shop.Entity.GetEquipmentItem(ProductTypeEnum.SellingPower, 1, productRepository);
                equipmentRepository.AddEquipmentItem(shop.Entity.EquipmentID.Value, item.ProductID, 1, 10);
            }

            equipmentRepository.SaveChanges();
        }

        private CompanyTypeEnum getCompanyTypeForProduct(ProductTypeEnum productType)
        {
            return productType.GetCompanyTypeForProduct();
        }

        private WorkTypeEnum getWorkTypeForProduct(ProductTypeEnum productType)
        {
            return productType.GetWorkType();
        }

        public CompanyEmployee EmployCitizen(EmployCitizenParameters pars)
        {
            var company = companyRepository
                .Include(c => c.CompanyEmployees)
                .FirstOrDefault(c => c.ID == pars.CompanyID);

            CompanyEmployee employee = new CompanyEmployee()
            {
                CitizenID = pars.CitizenID,
                MinHP = pars.ContractOffer == null ? company.DefaultMinHP : pars.ContractOffer.MinHP,
                Salary = (decimal)pars.Salary,
                StartDay = GameHelper.CurrentDay
            };

            if (pars.ContractOffer != null)
            {
                var contract = pars.ContractOffer;
                employee.JobContract = new JobContract()
                {
                    Length = contract.Length,
                    MinHP = contract.MinHP,
                    MinSalary = contract.MinSalary,
                    SigneeID = contract.SigneeID
                };
            }



            company.CompanyEmployees.Add(employee);
            jobOfferService.TakeJobOffer(pars.JobOfferID);

            companyRepository.SaveChanges();

            return employee;
        }

        public CompanyEmployee EmployCitizen(Company company, Citizen citizen, ContractJobOffer contract)
        {
            CompanyEmployee employee = new CompanyEmployee()
            {
                CitizenID = citizen.ID,
                MinHP = contract.MinHP,
                Salary = contract.MinSalary
            };

            employee.JobContract = new JobContract()
            {
                Length = contract.Length,
                MinHP = contract.MinHP,
                MinSalary = contract.MinSalary
            };

            company.CompanyEmployees.Add(employee);
            jobOfferService.TakeJobOffer(contract.JobOffer.ID);

            companyRepository.SaveChanges();

            return employee;
        }

        public bool CanLeaveJob(CompanyEmployee employee)
        {
            if (employee.GetJobType() == JobTypeEnum.Contracted)
            {
                var contract = employee.JobContract;

                return contract.AbusedByCompany;
            }
            return true;
        }

        public void LeaveJob(int citizenID, int companyID)
        {
            var employee = companyEmployeeRepository
                .First(e => e.CitizenID == citizenID
                         && e.CompanyID == companyID);
            companyEmployeeRepository.Remove(employee);
            companyEmployeeRepository.SaveChanges();
        }

        public bool IsWorkingAtCompany(int citizenID, int companyID)
        {
            var company = companyRepository.GetById(companyID);
            return company
                .CompanyEmployees
                .Where(e => e.CitizenID == citizenID)
                .Count() == 1;
        }

        public bool DoesHaveEnoughRawToWork(Citizen citizen, Company company)
        {
            var requiredRaws = GetNeededResourcesForWork(citizen, company);

            foreach (var raw in requiredRaws)
            {
                var magazineRaw = company.Entity.GetEquipmentItem(raw.RequiredProductType, raw.Quality, productRepository);
                if (magazineRaw.Amount < raw.Quantity)
                    return false;
            }

            return true;
        }

        //public bool DoesHave;

        public List<ProductRequirement> GetNeededResourcesForWork(Citizen citizen, Company company)
        {

            double productionPoints = GetProductionPoints(citizen, company);
            var requirements = productService.GetRequiredRaws((ProductTypeEnum)company.ProductID, company.Quality);

            double multiplier = (productionPoints);
            for (int i = 0; i < requirements.Count; ++i)
            {
                requirements[i].Quantity = (int)Math.Max(
                                                Math.Floor(requirements[i].Quantity * multiplier
                                                         ), 1);

            }

            return requirements;
        }

        public MethodResult Work(int citizenID, int companyID)
        {
            var result = new MethodResult();
            using (var t = transactionScopeProvider.CreateTransactionScope())
            {
                var company = companyRepository.GetById(companyID);
                var citizen = citizenRepository.First(c => c.ID == citizenID);
                var employee = company.CompanyEmployees.First(e => e.CitizenID == citizen.ID);


                produceProduct(company, citizen);
                updateCitizenAfterWork(company, citizen);
                result.Merge(payCashForWork(company, citizen, employee));


                if ((citizen.DayWorkedRow % 30) == 0)
                {
                    citizenService.ReceiveHardWorker(citizen);
                }
                var transaction = System.Transactions.Transaction.Current;

                if (transaction.TransactionInformation.Status == System.Transactions.TransactionStatus.Aborted)
                    throw new UserReadableException("You cannot work");

                company.InformedAboutNotEnoughSalaryToday = false;
                company.InformedAboutNotEnoughRawToday = false;

                companyRepository.SaveChanges();

                t.Complete();
            }

            return result;
        }

        private MethodResult payCashForWork(Company company, Citizen citizen, CompanyEmployee employee)
        {
            
            var currency = company.Region.Country.Currency;

            var transaction = new structs.Transaction()
            {
                Arg1 = "Job Salary",
                Arg2 = string.Format("{0} Paid {1}", company.Entity.Name, citizen.Entity.Name),
                DestinationEntityID = citizen.ID,
                SourceEntityID = company.ID,
                Money = new Money()
                {
                    Amount = employee.TodaySalary.Value,
                    Currency = currency
                },
                TransactionType = TransactionTypeEnum.Salary
            };

            var result = transactionService.MakeTransaction(transaction);

            if (result == enums.TransactionResult.NotEnoughMoney)
                return new MethodResult("Company does not have enough money");

            companyFinanceSummaryService.AddFinances(company, new SalaryCostFinance(employee.TodaySalary.Value, currency.ID));

            return MethodResult.Success;

        }

        private void updateCitizenAfterWork(Company company, Citizen citizen)
        {
            var employee = company.CompanyEmployees.First(e => e.CitizenID == citizen.ID);

            employee.TodayProduction = (decimal)GetProductionPoints(citizen, company);
            employee.TodayHP = citizen.HitPoints;
            employee.TodaySalary = (decimal)(GetTodaySalary(citizen, employee));

            var experience = getXPFromWork(company, citizen);
            var skillIncrease = citizenService.GetSkillIncreaseForWork(citizen.ID, (WorkTypeEnum)company.WorkTypeID);
            var lostHP = getHPForWork(citizen, company);

            citizen.HitPoints -= lostHP;
            citizen.LastActivityDay = GameHelper.CurrentDay;
            using (NoSaveChanges)
                citizenService.GrantExperience(citizen, getXPFromWork(company, citizen));

            citizenService.IncreaseSkill(citizen.ID, (WorkTypeEnum)company.WorkTypeID, skillIncrease);

            citizen.Worked = true;
            citizen.DayWorkedRow++;
        }

        public double GetTodaySalary(Citizen citizen, CompanyEmployee employee)
        {
            return (double)employee.Salary;
        }

        private void produceProduct(Company company, Citizen citizen)
        {
            using (NoSaveChanges)
            {
                var usedRaws = GetNeededResourcesForWork(citizen, company);

                foreach (var raw in usedRaws)
                {
                    equipmentService.RemoveProductsFromEquipment(raw.RequiredProductType, raw.Quantity, raw.Quality, company.Entity.Equipment);
                }


                company.Queue += (decimal)GetProductionPoints(citizen, company);
                int producedItems = (int)company.Queue;
                onProducedItems(company, producedItems);

                company.Queue -= producedItems;
            }
        }

        private void onProducedItems(Company company, int producedItems)
        {
            switch (company.GetCompanyType())
            {
                case CompanyTypeEnum.Developmenter:
                    //decimal additionalDevelopement = CalculateCreatedDevelopementForCreatedItems(producedItems);
                    var region = company.Region;
                    region.Development += producedItems;
                    break;
                case CompanyTypeEnum.Construction:
                    company.Construction.Progress += producedItems;

                    if (company.GetProducedProductType() == ProductTypeEnum.House)
                    {
                        var constructionService = DependencyResolver.Current.GetService<IConstructionService>();
                        var requiredProgress = constructionService.GetProgressNeededToBuild(ProductTypeEnum.House, company.Quality);

                        while (company.Construction.Progress >= requiredProgress)
                        {
                            equipmentService.GiveItem(company.GetProducedProductType(), 1, company.Quality, company.Entity.Equipment);
                            company.Construction.Progress -= requiredProgress;
                        }
                    }

                    break;
                default:
                    equipmentService.GiveItem(company.GetProducedProductType(), producedItems, company.Quality, company.Entity.Equipment);
                    break;
            }
        }

        public decimal CalculateCreatedDevelopementForCreatedItems(decimal producedItems)
        {
            return producedItems * 0.0875m * 0.22m / 10m;
        }

        private int getHPForWork(Citizen citizen, Company company)
        {
            switch (company.Quality)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 6;
                case 4:
                    return 10;
                case 5:
                    return 15;
            }
            throw new Exception("Wrong quality level of the company");
        }

        private int getXPFromWork(Company company, Citizen citizen)
        {
            switch (company.Quality)
            {
                case 1:
                    return (int)(Math.Max(1.0 * GetProductionPoints(citizen, company), 1.0));
                case 2:
                    return (int)(Math.Max(2.0 * GetProductionPoints(citizen, company), 1.0));
                case 3:
                    return (int)(Math.Max(4.0 * GetProductionPoints(citizen, company), 1.0));
                case 4:
                    return (int)(Math.Max(7.0 * GetProductionPoints(citizen, company), 1.0));
                case 5:
                    return (int)(Math.Max(11.0 * GetProductionPoints(citizen, company), 1.0));
            }
            throw new Exception("Wrong quality level of the company");
        }

        public double GetProductionPoints(Citizen citizen, Company company)
        {
            var factory = new ProductionPointsCalculatorFactory()
                .SetProductType(company.GetProducedProductType());

            var calculator = factory.Create();
            var args = new ProductionPointsCalculateArgs(citizen, company, regionService, regionRepository);

            return calculator.Calculate(args);
        }

        public double GetProductionPoints(ProductTypeEnum producedProduct, int hitPoints, double skill, double distance, int quality, int resourceQuality, double regionDevelopment, int peopleCount)
        {
            var factory = new ProductionPointsCalculatorFactory()
              .SetProductType(producedProduct);

            var calculator = factory.Create();
            var args = new ProductionPointsCalculateArgs()
            {
                Development = regionDevelopment,
                Distance = distance,
                PeopleCount = peopleCount,
                ProducedProduct = producedProduct,
                Quality = quality,
                ResourceQuality = resourceQuality,
                Skill = skill,
                HitPoints = hitPoints
            };

            return calculator.Calculate(args);
        }

        public ResourceTypeEnum GetResourceTypeForProduct(ProductTypeEnum productType)
        {
            return productType.GetResourceType();

        }


        public WorkStatistics GetWorkStatistics(int citizenID)
        {
            var employee = companyEmployeeRepository.FirstOrDefault(e => e.CitizenID == citizenID);

            if (employee == null)
                return null;

            if (employee.TodayProduction == null)
                return null;

            var currency = employee.Company.Entity.GetCurrentRegion().Country.Currency;
            var salary = new Money(currency, employee.TodaySalary.Value);

            return new WorkStatistics()
            {
                Production = (double)employee.TodayProduction.Value,
                Salary = salary,
                currency = currency
            };
        }

        public int GetHitPointsLostFromWork(ProductTypeEnum prodctType, int quality = 1)
        {
            return quality;
        }


        public MethodResult CanWork(Citizen citizen, Company company)
        {
            if (citizen == null)
                return new MethodResult("Citizen does not exist!");
            if (company == null)
                return new MethodResult("Company does not exist!");

            var employee = company.GetCompanyEmployee(citizen);

            if (employee == null)
                return new MethodResult("You do not work here!");

            if (employee.Citizen.Worked)
                return new MethodResult("You worked today!");

            var hpLostDueToWork = getHPForWork(citizen, company);

            if (citizen.HitPoints < hpLostDueToWork || citizen.HitPoints < employee.MinHP)
            {
                return new MethodResult("You do not have enough hit points to work!");
            }

            if (DoesHaveEnoughRawToWork(citizen, company) == false)
            {
                return new MethodResult("Company does not have enough raw");
            }

            var path = regionService.GetPathBetweenRegions(citizen.Entity.GetCurrentRegion(), company.Region, new DefaultRegionSelector(), new DefaultPassageCostCalculator(regionService));

            if (path == null)
                return new MethodResult("You are not able to go to the company. There is no route there!");


            if (DoesCompanyHaveEnoughMoneyForSalary(citizen, company) == false)
            {
                using (NoSaveChanges)
                {
                    InformAboutNotEnoughSalaryForWork(company, employee, GetTodaySalary(citizen, employee));
                    contractService.AddCompanyAbusement(employee);
                }
                companyRepository.SaveChanges();
                return new MethodResult("Company does not have enough money to pay you");
            }

            using (NoSaveChanges)
            {
                MethodResult result = DoesHaveEnoughSpaceForEmployeeWork(company, employee);
                if (result.IsError)
                {
                    companyRepository.SaveChanges();
                    return result;
                }
            }

            return MethodResult.Success;
        }

        public MethodResult DoesHaveEnoughSpaceForEmployeeWork(Company company, CompanyEmployee employee)
        {
            var equipment = company.Entity.Equipment;
            var neededSpace = GetNeededSpaceForWork(employee.Citizen, company);

            if (equipment.ItemCapacity - equipment.GetItemCount() < neededSpace)
            {
                InformAboutNotEnoughSpaceForWork(company, employee, neededSpace);
                contractService.AddCompanyAbusement(employee);
                return new MethodResult("Company does not have enough space for your created product!");
            }
            return MethodResult.Success;
        }


        public int GetNeededSpaceForWork(Citizen citizen, Company company)
        {
            var employee = company.CompanyEmployees
                .First(e => e.CitizenID == citizen.ID);

            var queue = (double)company.Queue;
            int producedAmount = (int)Math.Floor(queue + GetProductionPoints(citizen, company));

            var neededRaw = GetNeededResourcesForWork(citizen, company);
            int rawUsed = 0;
            foreach (var raw in neededRaw)
            {
                rawUsed += raw.Quantity;
            }

            return producedAmount - rawUsed;
        }

        public List<string> GetCannotLeaveJobReasons(CompanyEmployee employee)
        {
            throw new NotImplementedException();
        }

        public bool DoesCompanyHaveEnoughMoneyForSalary(Citizen citizen, Company company)
        {
            var employee = citizen.CompanyEmployee;

            var salary = GetTodaySalary(citizen, employee);

            var currency = company.Region.Country.Currency;

            var wallet = company.Entity.Wallet;

            var money = wallet.GetMoney(currency.ID, Persistent.Currencies.GetAll());

            if ((double)money.Amount < salary)
                return false;
            return true;
        }

        public CompanyRights GetCompanyRights(Company company, Entity currentEntity, Citizen loggedCitizen)
        {
            bool canUseCitizenRights = currentEntity.EntityID == loggedCitizen.ID;
            if (company.OwnerID == currentEntity.EntityID || company.ID == currentEntity.EntityID)
                return new CompanyRights(fullRights: true);

            if (canUseCitizenRights)
            {
                var manager = company.CompanyManagers.FirstOrDefault(r => r.EntityID == loggedCitizen.ID);

                if (manager != null)
                    return new CompanyRights(manager);
            }

            return new CompanyRights(fullRights: false);
        }

        public CompanyRights GetCompanyRights(Company company, Entity currentEntity)
        {
            if (company.OwnerID == currentEntity.EntityID || company.ID == currentEntity.EntityID)
                return new CompanyRights(fullRights: true);

            if (currentEntity.Is(EntityTypeEnum.Citizen))
            {
                var manager = company.CompanyManagers.FirstOrDefault(r => r.EntityID == currentEntity.EntityID);

                if (manager != null)
                    return new CompanyRights(manager);
            }

            return new CompanyRights(fullRights: false);
        }

        public bool DoesHaveRightTo(Company company, Entity currentEntity, Citizen loggedCitizen, CompanyRightsEnum right)
        {
            return GetCompanyRights(company, currentEntity, loggedCitizen)[right];
        }

        public bool DoesHaveRightTo(Company company, Entity currentEntity, CompanyRightsEnum right)
        {
            return GetCompanyRights(company, currentEntity)[right];
        }

        public void InformAboutNewEmployee(Company company, CompanyEmployee employee)
        {
            var currencyID = company.Region.Country.CurrencyID;
            var currency = Persistent.Currencies.First(c => c.ID == currencyID);

            string msg = string.Format("{0} accepted your job offer. From now on {0} is earning {1} {2} as {3} employee",
                employee.Citizen.Entity.Name,
                employee.Salary,
                currency.Symbol,
                employee.JobContractID == null ? "normal" : "contracted"
                );

            warningService.AddWarning(company.ID, msg);
        }

        public void InformAboutNotEnoughSpaceForWork(Company company, CompanyEmployee employee, int neededSpace)
        {

            if (company.InformedAboutNotEnoughRawToday)
                return;
            company.InformedAboutNotEnoughRawToday = true;

            var companyEq = company.Entity.Equipment;
            var companyLink = EntityLinkCreator.Create(company.Entity);
            var employeeLink = EntityLinkCreator.Create(employee.Citizen.Entity);

            var itemLimit = companyEq.ItemCapacity;
            var itemCount = companyEq.GetItemCount();

            string msg = string.Format("{0} wanted to work at {1} but there was not enough space in equipment({2}/{3}). {0} needed {2} equipment space",
                employeeLink,
                companyLink,
                itemCount,
                itemLimit);

            warningService.AddWarning(company.ID, msg);
        }

        public void InformAboutNotEnoughSalaryForWork(Company company, CompanyEmployee employee, double neededSalary)
        {
            if (company.InformedAboutNotEnoughSalaryToday)
                return;
            company.InformedAboutNotEnoughSalaryToday = true;

            var currency = Persistent.Currencies.GetById(company.Region.Country.CurrencyID);
            var money = company.Entity.Wallet.GetMoney(currency.ID, Persistent.Currencies.GetAll());
            var employeeLink = EntityLinkCreator.Create(employee.Citizen.Entity);
            var companyLink = EntityLinkCreator.Create(company.Entity);


            string msg = string.Format("{0} wanted to work at {1} but there was not enough money to pay {0}'s salary({2} {3}). {0} should receive {4} {3}.",
                employeeLink,
                companyLink,
                money.Amount,
                currency.Symbol,
                neededSalary);

            using (NoSaveChanges)
                warningService.AddWarning(company.ID, msg);
        }

        public void InformAboutNotEnoughRawToWork(Company company, CompanyEmployee employee, List<ProductRequirement> requirements)
        {
            var employeeLink = EntityLinkCreator.Create(employee.Citizen.Entity);
            var companyLink = EntityLinkCreator.Create(company.Entity);

            string msg = string.Format("{0} wanted to work at {1} but there was not enough resources. {0} Needed : ",
                employeeLink,
                companyLink);

            List<string> resources = new List<string>();

            foreach (var req in requirements)
                resources.Add(string.Format("{0} Q{1} {2}", req.Quantity, req.Quality, req.RequiredProductType.ToHumanReadable()));

            msg += string.Join(",", resources);

            warningService.AddWarning(company.ID, msg);
        }

        public static bool IsSellable(ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.UpgradePoints:
                case ProductTypeEnum.SellingPower:
                    return false;
            }

            return true;
        }

        public static List<ProductTypeEnum> GetSellableProducts(CompanyTypeEnum companyType)
        {
            switch (companyType)
            {
                case CompanyTypeEnum.Manufacturer:
                    return ProductGroups.Manufactured;
                case CompanyTypeEnum.Shop:
                    return ProductGroups.Consumables;
                case CompanyTypeEnum.Producer:
                    return ProductGroups.Raws;
            }

            return new List<ProductTypeEnum>();
        }

        public static bool CanSellProduct(CompanyTypeEnum companyType, ProductTypeEnum productType)
        {
            return GetSellableProducts(companyType).Contains(productType);
        }

        public MethodResult CanUpgradeCompany(Company company, Entity entity, Citizen loggedCitizen)
        {
            if (company == null)
                return new MethodResult("Company does not exist!");
            if (entity == null)
                return new MethodResult("Entity does not exist!");

            if (IsUpgradeAble(company) == false)
                return new MethodResult("Company is not upgradeable!");

            var rights = GetCompanyRights(company, entity, loggedCitizen);
            if (rights.CanUpgradeCompany == false)
                return new MethodResult("You have insufficient rights to upgrade company!");

            var goldNeeded = GetUpgradeCost(company);
            if (walletService.HaveMoney(company.Entity.WalletID, new Money(Persistent.Currencies.Gold, (decimal)goldNeeded)) == false)
                return new MethodResult($"Company does not have enough gold! It needs {goldNeeded} gold.");

            var consPoints = GetUpgradeConstructionPointsNeeded(company);

            if (equipmentService.HaveItem(company.Entity.Equipment, ProductTypeEnum.UpgradePoints, 1, consPoints).IsError)
                return new MethodResult($"Company does not have enough construction points! It needs {consPoints}.");

            return MethodResult.Success;
        }

        public void UpgradeCompany(Company company)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var cost = GetUpgradeCost(company);
                var consPoints = GetUpgradeConstructionPointsNeeded(company);

                equipmentService.RemoveProductsFromEquipment(ProductTypeEnum.UpgradePoints, consPoints, 1, company.Entity.Equipment);

                company.Quality++;
                company.Queue = 0m;

                if (transactionService.UgradeCompany(company, cost) == enums.TransactionResult.Success)
                    trs?.Complete();
            }
        }

        public double GetUpgradeCost(Company company)
        {
            switch (company.Quality)
            {
                case 1:
                    return 5;
                case 2:
                    return 20;
                case 3:
                    return 80;
                case 4:
                    return 200;
            }

            throw new ArgumentException();
        }

        public int GetUpgradeConstructionPointsNeeded(Company company)
        {
            switch (company.Quality)
            {
                case 1:
                    return 100;
                case 2:
                    return 400;
                case 3:
                    return 2500;
                case 4:
                    return 7500;
            }
            throw new ArgumentException();
        }

        public bool DoesUseQuality(Company company)
        {
            switch (company.GetCompanyType())
            {
                case CompanyTypeEnum.Shop:
                case CompanyTypeEnum.Producer:
                case CompanyTypeEnum.Developmenter:
                    return false;
                case CompanyTypeEnum.Manufacturer:
                case CompanyTypeEnum.Construction:
                    return true;
                default:
#if DEBUG
                    throw new Exception("Aaaa");
#else
                    return false;
#endif
            }

        }

        public bool IsUpgradeAble(Company company)
        {
            if (DoesUseQuality(company) == false)
                return false;

            if (company.Quality == 5)
                return false;

            switch (company.GetProducedProductType())
            {
                case ProductTypeEnum.UpgradePoints:
                    return false;
                case ProductTypeEnum.Fuel:
                    return false;
                case ProductTypeEnum.DefenseSystem:
                    return false;
                case ProductTypeEnum.Hospital:
                    return false;
                case ProductTypeEnum.Hotel:
                    return false;
                case ProductTypeEnum.House:
                    return false;

            }

            return true;
        }

        public CompanyManager GetManager(Company company, Entity entity)
        {
            return company.CompanyManagers.SingleOrDefault(m => m.ID == entity.EntityID);
        }

        public MethodResult CanAddManager(Company company, Entity addingEntity, Entity addedEntity, CompanyRights addedRights)
        {
            if (addingEntity == null)
                return new MethodResult("You do not exist!");
            if (addedEntity == null)
                return new MethodResult("Manager does not exist!");
            if (company == null)
                return new MethodResult("Company does not exist!");
            if (addedEntity.Is(EntityTypeEnum.Citizen) == false)
                return new MethodResult("Only citizens can be managers!");


            var addedCurrentRights = GetCompanyRights(company, addedEntity);
            if (addedCurrentRights.HaveAnyRights)
                return new MethodResult($"{addedEntity.Name} is actually a manager!");

            var rights = GetCompanyRights(company, addingEntity);


            if (rights.CanManageManagers == false)
                return new MethodResult("You cannot manage managers!");

            foreach (CompanyRightsEnum right in Enum.GetValues(typeof(CompanyRightsEnum)).Cast<CompanyRightsEnum>())
            {
                if (rights[right] == false && addedRights[right] == true)
                    return new MethodResult("You cannot give him more rights than you have!");
            }

            if (addedRights.Priority > rights.Priority)
                return new MethodResult("New manager cannot have bigger priority than you!");

            return MethodResult.Success;
        }

        public MethodResult CanModifyManager(Company company, Entity modifyingEntity, CompanyManager manager, CompanyRights newRights)
        {
            if (modifyingEntity == null)
                return new MethodResult("You do not exist!");
            if (manager == null)
                return new MethodResult("Manager does not exist!");
            if (company == null)
                return new MethodResult("Company does not exist!");

            var rights = GetCompanyRights(company, modifyingEntity);

            if (rights.CanManageManagers == false)
                return new MethodResult("You cannot manage managers!");

            foreach (CompanyRightsEnum right in Enum.GetValues(typeof(CompanyRightsEnum)).Cast<CompanyRightsEnum>())
            {
                if (rights[right] == false && newRights[right] == true)
                    return new MethodResult("You cannot give him more rights than you have!");
            }

            if (newRights.Priority > rights.Priority)
                return new MethodResult("New manager cannot have bigger priority than you!");

            return MethodResult.Success;
        }

        public void ModifyManager(CompanyManager manager, CompanyRights rights)
        {
            if (rights.HaveAnyRights == false)
                companyManagerRepository.Remove(manager);

            rights.FillEntity(ref manager);
            ConditionalSaveChanges(companyManagerRepository);
        }

        public MethodResult CanSeeManagers(Company company, Entity entity)
        {
            if (company == null)
                return new MethodResult("Company does not exist!");
            var rights = GetCompanyRights(company, entity);

            if (rights.HaveAnyRights == false)
                return new MethodResult("You need to be a manager in this company to do that!");

            return MethodResult.Success;
        }

        public MethodResult CanRemoveManager(CompanyManager manager, Entity byWho)
        {
            if (manager == null)
                return new MethodResult("Manager does not exist!");

            var managerRights = new CompanyRights(manager);
            var whoRights = GetCompanyRights(manager.Company, byWho);

            if (whoRights.CanManageManagers == false)
                return new MethodResult("You cannot manage managers!");
            if (whoRights.Priority <= managerRights.Priority)
                return new MethodResult("You need to have higher priority to be able to remove manager!");

            return MethodResult.Success;

        }

        public void RemoveManager(CompanyManager manager)
        {
            var companyLink = EntityLinkCreator.Create(manager.Company.Entity);
            string msg = $"You are no longer manager of {companyLink}.";
            using (NoSaveChanges)
                warningService.AddWarning(manager.EntityID, msg);

            companyManagerRepository.Remove(manager);
            companyManagerRepository.SaveChanges();
        }

        public virtual void RemoveJobOffersThatDoesNotMeetMinimalWage(decimal minimalWage, int countryID)
        {
            var offers = jobOfferRepository.GetJobOffersWithoutMinimalWage(minimalWage, countryID);

            var offersByCompanies = offers.GroupBy(o => o.CompanyID);


            using (NoSaveChanges)
            {
                foreach (var offer in offersByCompanies)
                {
                    InformCompanyAboutOfferRemovedDueToMinimalWage(offer.First(), offer.Count());
                }

                foreach (var offer in offers)
                {
                    jobOfferRepository.Remove(offer);
                }
            }

            jobOfferRepository.SaveChanges();

        }

        public virtual void InformCompanyAboutOfferRemovedDueToMinimalWage(JobOffer offer, int count)
        {
            var message = $"Your offer{PluralHelper.S(count)} {PluralHelper.Else(count, "was", "were")} deleted due to minimal wage change in country.";
            warningService.AddWarning(offer.CompanyID, message);
        }

        public bool CanUseRawMultiplier(Company company)
        {
            return company.GetWorkType() == WorkTypeEnum.Raw;
        }


        public bool CanUseQualityModifier(Company company)
        {
            return DoesUseQuality(company);
        }
    }
}
