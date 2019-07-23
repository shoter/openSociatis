using Common;
using Entities;
using Entities.enums;
using Entities.Models.Hospitals;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;
using WebServices.structs;

namespace WebServices
{
    public class TransactionsService : BaseService, ITransactionsService
    {
        IEntityRepository entitiesRepository;
        IWalletService walletsService;
        IConfigurationRepository configurationRepository;
        ITransactionLogRepository transactionLogRepository;
        IWalletRepository walletRepository;

        public TransactionsService(ITransactionLogRepository transactionLogRepository,
            IEntityRepository entitiesRepository, IWalletService walletsService, IConfigurationRepository configurationRepository,
            IWalletRepository walletRepository)
        {
            this.entitiesRepository = entitiesRepository;
            this.walletsService = walletsService;
            this.configurationRepository = configurationRepository;
            this.transactionLogRepository = transactionLogRepository;
            this.walletRepository = walletRepository;
        }
        public virtual TransactionResult MakeTransaction(Transaction transaction, bool useTransaction = true)
        {
            if (useTransaction)
            {
                var trs = transactionScopeProvider.CreateTransactionScope();
                var result = makeTransaction(transaction);
                trs?.Complete();
                trs?.Dispose();
                return result;

            }
            else
                return makeTransaction(transaction);


        }

        private TransactionResult makeTransaction(Transaction transaction)
        {
            Entity sourceEntity = null;
            Entity destinationEntity = null;
            if (transaction.DestinationEntityID.HasValue)
                destinationEntity = entitiesRepository.GetById(transaction.DestinationEntityID.Value);

            Wallet sourceWallet = null; 
            if (transaction.SourceWalletID.HasValue && transaction.TakeMoneyFromSource)
                sourceWallet = walletRepository.GetById(transaction.SourceWalletID.Value);
            else if (transaction.SourceEntityID.HasValue && transaction.TakeMoneyFromSource)
            {
                sourceEntity = entitiesRepository.GetById(transaction.SourceEntityID.Value);
                sourceWallet = sourceEntity.Wallet;
            }
            

            Wallet destinationWallet = null;
            if (transaction.DestinationWalletID.HasValue)
                destinationWallet = walletRepository.GetById(transaction.DestinationWalletID.Value);
            else if (destinationEntity != null)
                destinationWallet = destinationEntity.Wallet;
            

            if (sourceWallet != null)
            {
                var sourceMoney = sourceWallet.WalletMoneys.FirstOrDefault(wm => wm.CurrencyID == transaction.Money.Currency.ID);

                if (sourceMoney == null || sourceMoney.Amount < transaction.Money.Amount)
                    return TransactionResult.NotEnoughMoney;

                walletsService.AddMoney(sourceWallet.ID, -transaction.Money);
            }

            if (destinationWallet != null)
                walletsService.AddMoney(destinationWallet.ID, transaction.Money);


            TransactionLog log = new TransactionLog()
            {
                Amount = transaction.Money.Amount,
                CurrencyID = transaction.Money.Currency.ID,
                Date = DateTime.Now,
                Day = configurationRepository.GetCurrentDay(),
                SourceEntityID = sourceEntity?.EntityID,
                arg1 = transaction.Arg1,
                arg2 = transaction.Arg2,
                TransactionTypeID = (int)transaction.TransactionType,
                DestinationWalletID = destinationWallet?.ID,
                SourceWalletID = sourceWallet?.ID
            };

            if (destinationEntity != null)
                log.DestinationEntityID = destinationEntity.EntityID;

            transactionLogRepository.Add(log);
            transactionLogRepository.SaveChanges();

            return TransactionResult.Success;
        }

        public TransactionResult PayForWar(Country attacker, Country defender, IWarService warService)
        {
            var adminCurrency = Persistent.Currencies.Gold;
            var attEntity = attacker.Entity;
            var defEntity = defender.Entity;

            var adminMoney = new Money()
            {
                Amount = (decimal)warService.GetNeededGoldToStartWar(attacker, defender),
                Currency = adminCurrency
            };


            var adminTransaction = new Transaction()
            {
                Arg1 = "War fee",
                Arg2 = string.Format("{0}({1}) Attacked {2}({3})", attEntity.Name, attEntity.EntityID, defEntity.Name, defEntity.EntityID),
                DestinationEntityID = null,
                Money = adminMoney,
                SourceEntityID = attEntity.EntityID,
                TransactionType = TransactionTypeEnum.WarFee
            };

            return MakeTransaction(adminTransaction);
        }

        public TransactionResult PayForResistanceWar(Citizen attacker, Region defenderRegion, IWarService warService)
        {
            var adminCurrency = Persistent.Currencies.Gold;
            var defEntity = defenderRegion.Country.Entity;
            var attEntity = attacker.Entity;

            var adminMoney = warService.GetNeededMoneyToStartRessistanceWar(defenderRegion);

            var adminTransaction = new Transaction()
            {
                Arg1 = "Ressistance war fee",
                Arg2 = string.Format("{0}({1}) Attacked {2}({3})", attEntity.Name, attEntity.EntityID, defEntity.Name, defEntity.EntityID),
                DestinationEntityID = null,
                Money = adminMoney,
                SourceEntityID = attEntity.EntityID,
                TransactionType = TransactionTypeEnum.RessistanceWar
            };

            return MakeTransaction(adminTransaction, false);
        }

        public TransactionResult PayForResistanceBattle(Citizen attacker, Region defenderRegion, IWarService warService)
        {
            var adminCurrency = Persistent.Currencies.Gold;
            var defEntity = defenderRegion.Country.Entity;
            var attEntity = attacker.Entity;

            var adminMoney = warService.GetMoneyNeededToStartResistanceBattle(defenderRegion);

            var adminTransaction = new Transaction()
            {
                Arg1 = "Ressistance war fee",
                Arg2 = string.Format("{0}({1}) Attacked {2}({3})", attEntity.Name, attEntity.EntityID, defEntity.Name, defEntity.EntityID),
                DestinationEntityID = null,
                Money = adminMoney,
                SourceEntityID = attEntity.EntityID,
                TransactionType = TransactionTypeEnum.RessistanceWar
            };

            return MakeTransaction(adminTransaction, useTransaction: false);
        }

        public TransactionResult PayForBattleLoss(Country attacker, Country defender, double goldTaken)
        {
            var adminCurrency = Persistent.Currencies.Gold;
            var attEntity = attacker.Entity;
            var defEntity = defender.Entity;

            var adminMoney = new Money()
            {
                Amount = (decimal)goldTaken,
                Currency = adminCurrency
            };


            var adminTransaction = new Transaction()
            {
                Arg1 = "Battle Win",
                Arg2 = string.Format("{0}({1}) taken region from {2}({3})", attEntity.Name, attEntity.EntityID, defEntity.Name, defEntity.EntityID),
                DestinationEntityID = attEntity.EntityID,
                Money = adminMoney,
                SourceEntityID = defEntity.EntityID,
                TransactionType = TransactionTypeEnum.BattleWin
            };


            return MakeTransaction(adminTransaction, useTransaction: false);
        }

        public TransactionResult PayForJobOffer(Company company, Country receiver, int currencyID, decimal price)
        {
            var companyEntity = company.Entity;
            var countryEntity = receiver.Entity;

            var currency = Persistent.Currencies.GetById(currencyID);
            var money = new Money()
            {
                Amount = (decimal)price,
                Currency = currency
            };

            var transaction = new Transaction()
            {
                Arg1 = "Job Offer",
                Arg2 = string.Format("{0}({1}) created job offer in {2}({3})", companyEntity.Name, companyEntity.EntityID, countryEntity.Name, countryEntity.EntityID),
                DestinationEntityID = countryEntity.EntityID,
                Money = money,
                SourceEntityID = companyEntity.EntityID,
                TransactionType = TransactionTypeEnum.BattleWin
            };

            return MakeTransaction(transaction);
        }

        public TransactionResult PayForArticle(Article article, Entity payer, int currencyID, double articlePrice, double tax)
        {
            var newspaperEntity = article.Newspaper.Entity;
            var payingEntity = payer;

            var currency = Persistent.Currencies.GetById(currencyID);
            var money = new Money()
            {
                Amount = (decimal)articlePrice,
                Currency = currency
            };

            var transaction = new Transaction()
            {
                Arg1 = "Article price",
                Arg2 = string.Format("{0}({1}) paid for article in {2}({3})", payingEntity.Name, payingEntity.EntityID, newspaperEntity.Name, newspaperEntity.EntityID),
                DestinationEntityID = newspaperEntity.EntityID,
                Money = money,
                SourceEntityID = payingEntity.EntityID,
                TransactionType = TransactionTypeEnum.Article
            };
            MakeTransaction(transaction);

            var taxMoney = new Money()
            {
                Amount = (decimal)tax,
                Currency = currency
            };

            var country = article.Newspaper.Country.Entity;

            transaction = new Transaction()
            {
                Arg1 = "Article tax",
                Arg2 = string.Format("{0}({1}) paid article tax for {2}({3})", payingEntity.Name, payingEntity.EntityID, country.Name, country.EntityID),
                Money = taxMoney,
                DestinationEntityID = country.EntityID,
                SourceEntityID = payingEntity.EntityID,
                TransactionType = TransactionTypeEnum.Article
            };

            return MakeTransaction(transaction);
        }

        public TransactionResult PayForNewspaperCreation(Newspaper newspaper, Entity payer)
        {
            var newspaperEntity = newspaper.Entity;
            var countryEntity = newspaper.Country.Entity;
            var payingEntity = payer;

            var currency = Persistent.Countries.GetCountryCurrency(countryEntity.EntityID);
            var money = new Money()
            {
                Amount = newspaper.Country.CountryPolicy.NewspaperCreateCost,
                Currency = currency
            };

            var transaction = new Transaction()
            {
                Arg1 = "Newspaper creation",
                Arg2 = string.Format("{0}({1}) paid for newspaper creation to {2}({3})", payingEntity.Name, payingEntity.EntityID, countryEntity.Name, countryEntity.EntityID),
                DestinationEntityID = countryEntity.EntityID,
                Money = money,
                SourceEntityID = payingEntity.EntityID,
                TransactionType = TransactionTypeEnum.NewspaperCreation
            };
            MakeTransaction(transaction);

            var adminMoney = new Money()
            {
                Amount = configurationRepository.First().NewspaperCreationFee,
                Currency = Persistent.Currencies.GetById((int)CurrencyTypeEnum.Gold)
            };

            transaction = new Transaction()
            {
                Arg1 = "Newspaper creation",
                Arg2 = string.Format("{0}({1}) paid for newspaper creation to admin", payingEntity.Name, payingEntity.EntityID),
                Money = adminMoney,
                SourceEntityID = payingEntity.EntityID,
                TransactionType = TransactionTypeEnum.NewspaperCreation
            };

            return MakeTransaction(transaction);
        }

        public TransactionResult TransferMoneyFromCountryToCompany(Money money, Company destination, Country source)
        {

            var companyEntity = destination.Entity;
            var countryEntity = source.Entity;

            var transaction = new Transaction()
            {
                Arg1 = "Money transfer",
                Arg2 = string.Format("{0}({1}) transfered money to {2}({3})", countryEntity.Name, countryEntity.EntityID, companyEntity.Name, companyEntity.EntityID),
                SourceEntityID = countryEntity.EntityID,
                DestinationEntityID = companyEntity.EntityID,
                Money = money,
                TransactionType = TransactionTypeEnum.MoneyTransfer,
                TakeMoneyFromSource = false
            };
            return MakeTransaction(transaction);
        }

        public TransactionResult MakeGift(Entity source, Entity destination, Money money)
        {
            var transaction = new Transaction()
            {
                Arg1 = "Money transfer",
                Arg2 = string.Format("{0}({1}) made gift to {2}({3})", source.Name, source.EntityID, destination.Name, destination.EntityID),
                DestinationEntityID = destination.EntityID,
                Money = money,
                SourceEntityID = source.EntityID,
                TransactionType = TransactionTypeEnum.Gift
            };
            return MakeTransaction(transaction);
        }

        public TransactionResult UgradeCompany(Company company, double goldCost)
        {
            var money = new Money(Persistent.Currencies.Gold, (decimal)goldCost);
            var companyEntity = company.Entity;
            var transaction = new Transaction()
            {
                Arg1 = "Money transfer",
                Arg2 = string.Format("{0}({1}) has been upgraded", companyEntity.Name, companyEntity.EntityID),
                Money = money,
                SourceEntityID = companyEntity.EntityID,
                TransactionType = TransactionTypeEnum.CompanyUpgrade
            };
            return MakeTransaction(transaction);
        }

        public TransactionResult PayForHealing(Hospital hospital, Citizen citizen, HealingPrice healinPrice)
        {
            var hospitalEntity = hospital.Company.Entity;
            var citizenEntity = citizen.Entity;

            var currency = Persistent.Currencies.GetById(healinPrice.CurrencyID);

            var money = new Money(currency, healinPrice.Cost.Value);

            var transaction = new Transaction()
            {
                Arg1 = "Healing",
                Arg2 = string.Format("{0}({1}) paid for healing in {2}({3})",
                citizenEntity.Name, citizenEntity.EntityID, hospitalEntity.Name, hospitalEntity.EntityID),
                DestinationEntityID = hospitalEntity.EntityID,
                SourceEntityID = citizenEntity.EntityID,
                Money = money,
                TransactionType = TransactionTypeEnum.Healing
            };

            return MakeTransaction(transaction, false);
        }

        public TransactionResult PayForMPPOffer(Country proposing, Country other, decimal goldCost)
        {
            var money = new Money(Persistent.Currencies.Gold, (decimal)goldCost);
            var proposingEntity = proposing.Entity;
            var otherEntity = other.Entity;

            var transaction = new Transaction()
            {
                Arg1 = "Money transfer",
                Arg2 = string.Format("{0}({1}) proposed MPP {2}({3})"
                , proposingEntity.Name, proposingEntity.EntityID, otherEntity.Name, otherEntity.EntityID),
                Money = money,
                SourceEntityID = proposingEntity.EntityID,
                TransactionType = TransactionTypeEnum.PayForMPP
            };
            return MakeTransaction(transaction);
        }

        public TransactionResult GetGoldForDeclineMPP(Country proposing, Country other, decimal goldCost)
        {
            var money = new Money(Persistent.Currencies.Gold, (decimal)goldCost);
            var proposingEntity = proposing.Entity;
            var otherEntity = other.Entity;

            var transaction = new Transaction()
            {
                Arg1 = "Money transfer",
                Arg2 = string.Format("{2}({3}) declined MPP of {0}({1})"
                , proposingEntity.Name, proposingEntity.EntityID, otherEntity.Name, otherEntity.EntityID),
                Money = money,
                DestinationEntityID = proposingEntity.EntityID,
                TransactionType = TransactionTypeEnum.MPPReturnDecline
            };
            return MakeTransaction(transaction);
        }

        public TransactionResult PrintMoney(Country country, Money money)
        {
            var countryEntity = country.Entity;

            var transaction = new Transaction()
            {
                Arg1 = "Money transfer",
                Arg2 = string.Format("{0}({1}) printed money"
                , countryEntity.Name, countryEntity.EntityID),
                Money = money,
                DestinationEntityID = countryEntity.EntityID,
                TransactionType = TransactionTypeEnum.PrintMoney
            };
            return MakeTransaction(transaction);
        }
    }
}
