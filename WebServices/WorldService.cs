using Common;
using Common.Logging;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using WebServices.Helpers;

namespace WebServices
{
    public class WorldService : BaseService, IWorldService
    {
        ICitizenRepository citizenRepository;
        IConfigurationRepository configurationRepository;
        IEquipmentRepository equipmentRepository;
        ICompanyEmployeeRepository companyEmployeeRepository;
        IContractRepository contractRepository;
        IContractService contractService;
        IPartyService partyService;
        ICongressCandidateService congressCandidateService;
        ICongressVotingService congressVotingService;
        ICountryService countryService;
        IBattleService battleService;
        IRegionService regionService;
        ICompanyService companyService;
        private readonly IWalletService walletService;
        private readonly IWarService warService;
        private readonly IEmbargoService embargoService;
        private readonly IEmployeeService employeeService;
        private readonly IMPPService mppService;
        private readonly INewDayRepository newDayRepository;
        private readonly IHotelService hotelService;
        private readonly IHouseService houseService;
        public WorldService(ICompanyEmployeeRepository companyEmployeeRepository, IContractRepository contractRepository, IContractService contractService,
            ICitizenRepository citizenRepository, IConfigurationRepository configurationRepository, IEquipmentRepository equipmentRepository,
            IPartyService partyService, ICongressCandidateService congressCandidateService, ICongressVotingService congressVotingService,
            ICountryService countryService, IBattleService battleService, IRegionService regionService, ICompanyService companyService,
            IEmbargoService embargoService, IWarService warService, IEmployeeService employeeService, IMPPService mppService, IWalletService walletService,
            INewDayRepository newDayRepository, IHotelService hotelService, IHouseService houseService)
        {
            this.citizenRepository = citizenRepository;
            this.configurationRepository = configurationRepository;
            this.equipmentRepository = equipmentRepository;
            this.companyEmployeeRepository = companyEmployeeRepository;
            this.contractRepository = contractRepository;
            this.contractService = contractService;
            this.partyService = partyService;
            this.congressCandidateService = congressCandidateService;
            this.congressVotingService = congressVotingService;
            this.countryService = countryService;
            this.battleService = battleService;
            this.regionService = regionService;
            this.companyService = companyService;
            this.embargoService = embargoService;
            this.warService = warService;
            this.employeeService = employeeService;
            this.mppService = mppService;
            this.walletService = Attach(walletService);
            this.newDayRepository = newDayRepository;
            this.hotelService = hotelService;
            this.houseService = houseService;

            citizenRepository.SetTimeout(300);
            equipmentRepository.SetTimeout(300);

        }

        public void ProcessDayChange()
        {
            if (GameHelper.IsDayChange)
                return;

            try
            {
                using (var trs = transactionScopeProvider.CreateTransactionScope(IsolationLevel.Serializable, TimeSpan.FromMinutes(8)))
                {
                    GameHelper.IsDayChange = true;
                    processCitizens();

                    var configuration = configurationRepository.GetConfiguration();
                    DebugLogger log = new DebugLogger();

#if !DEBUG
                if ((DateTime.Now - configuration.LastDayChange).TotalHours < 15)
                    throw new Exception("Last day change was less than 15 hrs ago!");
#endif

                    contractService.ProcessDayChange();
                    congressCandidateService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("congressCandidateService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    partyService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("partyService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    countryService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("countryService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    congressVotingService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("congressVotingService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    battleService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("battleService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    regionService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("regionService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    companyService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("companyService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    embargoService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("embargoService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    hotelService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("hotelService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    houseService.ProcessDayChange(GameHelper.CurrentDay + 1);
                    log.Log("hotelService - {0}", System.Transactions.Transaction.Current.TransactionInformation.Status);

                    employeeService.ProcessDayChange(GameHelper.CurrentDay + 1);

                    warService.ProcessDayChange(GameHelper.CurrentDay + 1);

                    mppService.ProcessDayChange(GameHelper.CurrentDay + 1);

                    configuration.CurrentDay++;
                    configuration.LastDayChange = DateTime.Now.Date;

                    newDayRepository.AddInformationAboutNewDay(GameHelper.CurrentDay);

                    GameHelper.CurrentDay = configuration.CurrentDay;
                    GameHelper.LastDayChangeTime = configuration.LastDayChange;
                    configurationRepository.SaveChanges();

                    trs.Complete();
                }
            }
            finally
            {
                GameHelper.IsDayChange = false;
            }

            ReloadPersistent();
        }

        public void ReloadPersistent()
        {
            Persistent.Init();
        }

        public bool CanChangeDay()
        {
            if ((DateTime.Now - GameHelper.LastDayChangeTime)?.TotalHours > 24)
            {
                return true;
            }

            return false;
        }

        private void processCitizens()
        {
            var citizens = citizenRepository.GetAll().ToList();

            for (int i = 0; i < citizens.Count; ++i)
            {
                var citizen = citizens[i];

                if (citizen.Worked == false)
                    citizen.DayWorkedRow = 0;

                citizen.Worked = false;

                if (citizen.HasFood())
                {
                    var bread = citizen.GetBestBread();
                    equipmentRepository.RemoveEquipmentItem(citizen.Entity.EquipmentID.Value, bread.ProductID, bread.Quality);
                    citizen.AddHealth(bread.Quality * 2);
                }
                else
                {
                    citizen.HitPoints -= 10;
                    if (citizen.HitPoints < 0)
                        citizen.HitPoints = 0;
                }
                citizen.Trained = false;
                citizen.UsedHospital = false;
                citizen.DrankTeas = 0;
                using (NoSaveChanges)
                    walletService.AddMoney(citizen.Entity.WalletID, new structs.Money((int)CurrencyTypeEnum.Gold, 0.5m));
            }
            citizenRepository.SaveChanges();

        }
    }
}
