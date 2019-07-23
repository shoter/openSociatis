using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Extensions;
using Entities.Repository;
using Entities.enums;
using WebServices.enums;
using WebServices.structs;
using WebServices.Helpers;
using Common;

namespace WebServices
{
    public class EmbargoService : BaseService, IEmbargoService
    {
        private readonly ICountryRepository countryRepository;
        private readonly IEmbargoRepository embargoRepository;
        private readonly IWarningService warningService;
        private readonly Entities.Repository.IWalletRepository walletRepository;
        private readonly ITransactionsService transactionService;

        public EmbargoService(ICountryRepository countryRepository, IEmbargoRepository embargoRepository, IWarningService warningService, Entities.Repository.IWalletRepository walletRepository,
            ITransactionsService transactionService)
        {
            this.countryRepository = countryRepository;
            this.embargoRepository = embargoRepository;
            this.warningService = warningService;
            this.walletRepository = walletRepository;
            this.transactionService = transactionService;
        }

        public bool CanDeclareEmbargo(Country declaringCountry, Country embargoedCountry, Entity issuingEntity)
        {
            if (embargoRepository.Any(e => e.CreatorCountryID == declaringCountry.ID && e.EmbargoedCountryID == embargoedCountry.ID))
                return false;

            return CanDeclareEmbargo(declaringCountry, issuingEntity);
        }

        public bool CanDeclareEmbargo(Country declaringCountry, Entity issuingEntity)
        {
            return declaringCountry.PresidentID == issuingEntity.EntityID;
        }

        public void DeclareEmbargo(Country declaringCountry, Country embargoedCountry)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var embargo = new Embargo()
                {
                    Active = true,
                    CreatorCountryID = declaringCountry.ID,
                    EmbargoedCountryID = embargoedCountry.ID,
                    StartDay = GameHelper.CurrentDay,
                    StartTime = DateTime.Now
                };

                embargoRepository.Add(embargo);
                embargoRepository.SaveChanges();

                string message = string.Format("Your country was embargoed by {0}", declaringCountry.Entity.Name);
                warningService.AddWarning(embargoedCountry.ID, message);

                MakeEmbargoCostTransaction(embargo, false);
                trs.Complete();
            }
        }

        public double GetEmbargoCost(Embargo embargo)
        {
            return GetEmbargoCost(embargo.CreatorCountry, embargo.EmbargoedCountry);
        }

        public double GetEmbargoCost(Country declareCountry, Country embargoedCountry)
        {
            double declareDevelopement = getDevelopementSumForCountry(declareCountry);
            double embargoedDevelopement = getDevelopementSumForCountry(embargoedCountry);

            int declareRegions = declareCountry.Regions.Count();

            return Math.Round(Math.Max(((embargoedDevelopement + 1) / (embargoedDevelopement + declareDevelopement + 1)) / 10, 0.01) * declareRegions, 2);
        }

        private static double getDevelopementSumForCountry(Country declareCountry)
        {
            double developement = 0.0;
            if (declareCountry.Regions.Count > 0)
                developement = (double)declareCountry.Regions.Select(r => r.Development).Sum();
            return developement;
        }

        public bool CanCancelEmbargo(Embargo embargo, Entity entity)
        {
            if (embargo.CreatorCountry.PresidentID == entity.EntityID)
                return true;
            return false;
        }

        public void CancelEmbargo(Embargo embargo)
        {
            var message = string.Format("You no longer have embargo with {0}", embargo.CreatorCountry.Entity.Name);
            warningService.AddWarning(embargo.EmbargoedCountryID, message);

            embargo.Active = false;
            embargo.EndDay = GameHelper.CurrentDay;
            embargo.EndTime = DateTime.Now;
            
            embargoRepository.SaveChanges();
        }

        public void ProcessDayChange(int currentDay)
        {
            var embargoes = embargoRepository.GetAllActiveEmbargoes();

            foreach (var embargo in embargoes)
            {
                if(CanUpkeepEmbargo(embargo) == false)
                {
                    var message = string.Format("You did not have enough gold to keep your embargo with {0}", embargo.EmbargoedCountry.Entity.Name);
                    CancelEmbargo(embargo);
                    warningService.AddWarning(embargo.CreatorCountryID, message);
                }
                else
                {
                    MakeEmbargoCostTransaction(embargo);
                }
            }

            embargoRepository.SaveChanges();
        }

        public bool CanUpkeepEmbargo(Country issuingCountry, Country embargoedCountry)
        {
            var cost = GetEmbargoCost(issuingCountry, embargoedCountry);
            return CanUpkeepEmbargo(cost, issuingCountry.Entity.WalletID);
        }

        public bool CanUpkeepEmbargo(Embargo embargo)
        {
            var cost = GetEmbargoCost(embargo);
            return CanUpkeepEmbargo(cost, embargo.CreatorCountry.Entity.WalletID);
        }

        public bool CanUpkeepEmbargo(double cost, int walletID)
        {
            if (walletRepository.HaveMoney(walletID, CurrencyTypeEnum.Gold, cost) == false)
            {
                return false;
            }
            return true;
        }

        public TransactionResult MakeEmbargoCostTransaction(Embargo embargo, bool useSqlTransaction = false)
        {
            var cost = GetEmbargoCost(embargo);

            var payingEntity = embargo.CreatorCountry.Entity;

            var money = new Money()
            {
                Amount = (decimal)cost,
                Currency = Persistent.Currencies.GetById((int)CurrencyTypeEnum.Gold)
            };

            var transaction = new Transaction()
            {
                Arg1 = "Embargo upkeep",
                Arg2 = string.Format("{0}({1}) paid embargo upkeep", payingEntity.Name, payingEntity.EntityID),
                Money = money,
                SourceEntityID = payingEntity.EntityID,
                TransactionType = TransactionTypeEnum.Embargo
            };

            return transactionService.MakeTransaction(transaction, useSqlTransaction);

        }
    }
}
