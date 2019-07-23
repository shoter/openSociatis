using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using WebServices.structs.Params.MonetaryMarket;
using System.Diagnostics;
using Entities.Repository;
using Entities.enums;
using Common;
using WebServices.structs;
using System.Transactions;
using WebServices.Helpers;
using WebServices.structs.MonetaryMarket;

namespace WebServices
{
    public class MonetaryMarketService : BaseService, IMonetaryMarketService
    {
        private readonly IMonetaryOfferRepository monetaryOfferRepository;
        private readonly IMonetaryTransactionRepository monetaryTransactionRepository;
        private readonly ITransactionsService transactionService;
        private readonly IEntityRepository entityRepository;
        private readonly ICountryRepository countryRepository;
        

        public MonetaryMarketService(IMonetaryOfferRepository monetaryOfferRepository, IMonetaryTransactionRepository monetaryTransactionRepository, 
            ITransactionsService transactionService, IEntityRepository entityRepository, ICountryRepository countryRepository)
        {
            this.monetaryOfferRepository = monetaryOfferRepository;
            this.monetaryTransactionRepository = monetaryTransactionRepository;
            this.transactionService = Attach(transactionService);
            this.entityRepository = entityRepository;
            this.countryRepository = countryRepository;
        }


        public MonetaryOffer CreateOffer(CreateMonetaryOfferParam param)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                if (param.IsValid == false)
                    throw new ArgumentException("Invalid CreateMonetaryOfferParam");

                int usedCurrencyID = getUsedCurrencyID(param);

                var country = Persistent.Countries.FirstOrDefault(c => c.CurrencyID == usedCurrencyID);
                CountryPolicy policy = null;
                if(country != null)
                    policy = countryRepository.GetCountryPolicyById(country.ID);
                var entity = param.Seller;

                var offer = new MonetaryOffer()
                {
                    Amount = param.Amount.Value,
                    BuyCurrencyID = param.BuyCurrency.ID,
                    OfferTypeID = (int)param.OfferType,
                    Rate = (decimal)param.Rate.Value,
                    SellCurrencyID = param.SellCurency.ID,
                    SellerID = param.Seller.EntityID,
                    Tax = policy?.MonetaryTaxRate ?? 0,
                    MinimumTax = policy?.MinimumMonetaryTax ?? 0,
                    TaxReservedMoney = (decimal)CalculateTax(param.Amount.Value * (param.OfferType == MonetaryOfferTypeEnum.Buy ? param.Rate.Value : 1), ((double?)policy?.MonetaryTaxRate) ?? 0.0, ((double?)policy?.MinimumMonetaryTax) ?? 0.0),
                    OfferReservedMoney = (decimal)(param.Amount.Value * (param.OfferType == MonetaryOfferTypeEnum.Buy ? param.Rate.Value : 1)),
                    AmountSold = 0,
                    ValueOfSold = 0
                };

                ReserveMoneyFromEntity(entity, offer);
                monetaryOfferRepository.Add(offer);
                tryToBuySell(offer);
                monetaryOfferRepository.SaveChanges();
                transaction.Complete();
                return offer;
            }

        }

        public void ReserveMoneyFromEntity(Entity entity, MonetaryOffer offer)
        {
            var overallReservedMoney = (double)(offer.TaxReservedMoney + offer.OfferReservedMoney);
            var currency = GetUsedCurrency(offer);
            var money = new Money()
            {
                Amount = (decimal)overallReservedMoney,
                Currency = currency
            };

            var transaction = new structs.Transaction()
            {
                Arg1 = "Monetary tax",
                Arg2 = string.Format("{0}({1}) Reserved money for MM transaction #{2} - tax {3} + reserved {4}", entity.Name, entity.EntityID, offer.ID, offer.TaxReservedMoney, offer.OfferReservedMoney),
                Money = money,
                SourceEntityID = entity.EntityID,
                TransactionType = TransactionTypeEnum.MonetaryTax
            };
            transactionService.MakeTransaction(transaction, useSqlTransaction: false);
        }

        private int getUsedCurrencyID(CreateMonetaryOfferParam param)
        {
            return GetUsedCurrencyID(param.OfferType.Value, param.SellCurency.ID, param.BuyCurrency.ID);
        }

        public int GetUsedCurrencyID(MonetaryOffer offer)
        {
            return GetUsedCurrencyID((MonetaryOfferTypeEnum)offer.OfferTypeID, offer.SellCurrencyID, offer.BuyCurrencyID);
        }

        public int GetUsedCurrencyID(MonetaryOfferTypeEnum offerType, int sellCurrencyID, int buyCurrencyID)
        {
            return offerType == MonetaryOfferTypeEnum.Sell ? buyCurrencyID : sellCurrencyID;
        }

        private Currency GetUsedCurrency(MonetaryOffer offer)
        {
            return Persistent.Currencies.GetById(GetUsedCurrencyID(offer));
        }

        private void tryToBuySell(MonetaryOffer offer)
        {
            var entity = entityRepository.GetById(offer.SellerID);

            MonetaryTransaction lastTransaction = null;

            if (offer.OfferTypeID == (int)MonetaryOfferTypeEnum.Buy)
            {
                var suitableSellOffers = monetaryOfferRepository.
                    Where(o => o.Rate <= offer.Rate && 
                    o.OfferTypeID == (int)MonetaryOfferTypeEnum.Sell &&
                    o.SellCurrencyID == offer.SellCurrencyID &&
                    o.BuyCurrencyID == offer.BuyCurrencyID
                );

                if(suitableSellOffers.Any())
                {
                    var offers = suitableSellOffers.OrderBy(o => o.Rate).ToList();
                    foreach (var sellOffer in offers)
                    {
                        int boughtAmount = makeTransaction(offer, entity, sellOffer);

                        MonetaryTransaction transaction = new MonetaryTransaction()
                        {
                            Date = DateTime.Now,
                            Day = GameHelper.CurrentDay,
                            Amount = boughtAmount,
                            Rate = sellOffer.Rate,
                            BuyerCurrencyID = offer.BuyCurrencyID,
                            SellerCurrencyID = offer.SellCurrencyID,
                            SellerID = sellOffer.SellerID,
                            BuyerID = offer.SellerID,
                            SellerTax = 0,
                            BuyerTax = 0
                        };

                        lastTransaction = transaction;


                        if (sellOffer.Amount == 0)
                            RemoveOffer(sellOffer, ref transaction);
                        else
                            transaction.MonetaryOffers.Add(sellOffer);

                        if (offer.Amount == 0)
                        {
                            RemoveOffer(offer, ref transaction);
                            monetaryTransactionRepository.Add(transaction);
                            offer = null;
                            break;

                        }
                        monetaryTransactionRepository.Add(transaction);
                    }
                }
            }
            else
            {
                var suitableSellOffers = monetaryOfferRepository.
                    Where(o => o.Rate >= offer.Rate &&
                    o.OfferTypeID == (int)MonetaryOfferTypeEnum.Buy &&
                    o.SellCurrencyID == offer.SellCurrencyID &&
                    o.BuyCurrencyID == offer.BuyCurrencyID
                );

                if (suitableSellOffers.Any())
                {
                    var offers = suitableSellOffers.OrderBy(o => o.Rate).ToList();
                    foreach (var buyOffer in offers)
                    {
                        int boughtAmount = makeTransaction(buyOffer, buyOffer.Entity, offer );

                        MonetaryTransaction transaction = new MonetaryTransaction()
                        {
                            Date = DateTime.Now,
                            Day = GameHelper.CurrentDay,
                            Amount = boughtAmount,
                            BuyerID = buyOffer.SellerID,
                            Rate = offer.Rate,
                            BuyerCurrencyID = offer.BuyCurrencyID,
                            SellerCurrencyID = offer.SellCurrencyID,
                            SellerID = offer.SellerID,
                            SellerTax = 0,
                            BuyerTax = 0
                        };

                        lastTransaction = transaction;

                        if (buyOffer.Amount == 0)
                            RemoveOffer(buyOffer, ref transaction);
                        else
                            transaction.MonetaryOffers.Add(buyOffer);

                        if (offer.Amount == 0)
                        {
                            RemoveOffer(offer, ref transaction);
                            monetaryTransactionRepository.Add(transaction);
                            offer = null;
                            break;

                        }

                        monetaryTransactionRepository.Add(transaction);
                    }
                }
            }

            if (offer != null)
            {
                offer.LastTransaction = lastTransaction;
            }
        }

        private int makeTransaction(MonetaryOffer buyOffer, Entity buyerEntity, MonetaryOffer sellOffer)
        {
            var boughtAmount = 0;
            if (buyOffer.Amount >= sellOffer.Amount)
                boughtAmount = sellOffer.Amount;
            else
                boughtAmount = buyOffer.Amount;

            buyOffer.Amount -= boughtAmount;
            buyOffer.AmountSold += boughtAmount;
            sellOffer.Amount -= boughtAmount;
            sellOffer.AmountSold += boughtAmount;

            var soldMoney = (double)(boughtAmount * sellOffer.Rate);
            buyOffer.ValueOfSold += (decimal)soldMoney;
            sellOffer.ValueOfSold += boughtAmount;
            buyOffer.OfferReservedMoney -= (decimal)soldMoney;
            sellOffer.OfferReservedMoney -= boughtAmount;

            makeMarketTransaction(buyOffer, buyerEntity, sellOffer, boughtAmount);
            return boughtAmount;
        }

        private void makeMarketTransaction(MonetaryOffer buyOffer, Entity buyerEntity, MonetaryOffer sellOffer, int boughtAmount)
        {
            var money = new Money()
            {
                Amount = boughtAmount * sellOffer.Rate,
                Currency = Persistent.Currencies.GetById(buyOffer.SellCurrencyID)
            };

            var sellEntity = entityRepository.GetById(sellOffer.SellerID);

            var sociatisTransaction = new structs.Transaction()
            {
                Arg1 = "Monetary transaction",
                Arg2 = string.Format("buyer - {0}({1}) MM sell transaction - seller {2}({3})", buyerEntity.Name, buyerEntity.EntityID, sellEntity.Name, sellEntity.EntityID),
                Money = money,
                DestinationEntityID = sellEntity.EntityID,
                TransactionType = TransactionTypeEnum.MonetaryMarket
            };
            transactionService.MakeTransaction(sociatisTransaction, useSqlTransaction: false);

            money = new Money()
            {
                Amount = boughtAmount,
                Currency = Persistent.Currencies.GetById(buyOffer.BuyCurrencyID)
            };

            sociatisTransaction = new structs.Transaction()
            {
                Arg1 = "Monetary transaction",
                Arg2 = string.Format("buyer - {0}({1}) MM buy transaction - seller {2}({3})", buyerEntity.Name, buyerEntity.EntityID, sellEntity.Name, sellEntity.EntityID),
                Money = money,
                DestinationEntityID = buyerEntity.EntityID,
                TransactionType = TransactionTypeEnum.MonetaryMarket
            };
            transactionService.MakeTransaction(sociatisTransaction, useSqlTransaction: false);
        }

        public void RemoveOffer(MonetaryOffer offer, ref MonetaryTransaction monetaryTransaction)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                payMonetaryOfferTax(offer, ref monetaryTransaction);
                payBackReservedMoney(offer);
                offer.LastTransaction = null;
                monetaryOfferRepository.Remove(offer);
                monetaryOfferRepository.SaveChanges();
                transaction?.Complete();
            }
        }

        public void OnlyRemoveOffer(MonetaryOffer offer)
        {
            var lastTransaction = offer.LastTransaction;
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                if (lastTransaction != null) 
                    payMonetaryOfferTax(offer, ref lastTransaction);
                payBackReservedMoney(offer);
                monetaryOfferRepository.Remove(offer);
                offer.LastTransaction = null;
                monetaryOfferRepository.SaveChanges();
                transaction?.Complete();
            }
        }

        private void payBackReservedMoney(MonetaryOffer offer)
        {
            var giveBackMoneyAmount = (double)(offer.TaxReservedMoney + offer.OfferReservedMoney);
            var currency = GetUsedCurrency(offer);
            var offerEntity = offer.Entity;
            var money = new Money()
            {
                Amount = (decimal)giveBackMoneyAmount,
                Currency = currency
            };

            var sociatisTransaction = new structs.Transaction()
            {
                Arg1 = "Monetary tax",
                Arg2 = string.Format("{0}({1}) got his money back from monetary offer", offerEntity.Name, offerEntity.EntityID),
                Money = money,
                DestinationEntityID = offerEntity.EntityID,
                TransactionType = TransactionTypeEnum.MonetaryTax
            };

            transactionService.MakeTransaction(sociatisTransaction, useSqlTransaction: false);
        }

        private void payMonetaryOfferTax(MonetaryOffer offer, ref MonetaryTransaction transaction)
        {
            var tax = CalculateTax((double)offer.ValueOfSold, getTaxRate(offer));
            addTaxToTransaction(offer, transaction, tax);
            var currency = GetUsedCurrency(offer);

            var countryEntity = Persistent.Countries.FirstOrDefault(c => c.CurrencyID == currency.ID)?.Entity;
            var payingEntity = offer.Entity;
            var money = new Money()
            {
                Amount = (decimal)tax,
                Currency = currency
            };

            var sociatisTransaction = new structs.Transaction()
            {
                Arg1 = "Monetary tax",
                Arg2 = string.Format("{0}({1}) paid monetary tax in {2}({3})", payingEntity.Name, payingEntity.EntityID, countryEntity?.Name, countryEntity?.EntityID),
                Money = money,
                DestinationEntityID = countryEntity?.EntityID,
                TransactionType = TransactionTypeEnum.MonetaryTax
            };
            offer.TaxReservedMoney -= (decimal)tax;
            transactionService.MakeTransaction(sociatisTransaction, useSqlTransaction: false);
        }

        private static void addTaxToTransaction(MonetaryOffer offer, MonetaryTransaction transaction, double tax)
        {
            if (offer.OfferTypeID == (int)MonetaryOfferTypeEnum.Buy)
                transaction.BuyerTax += (decimal)tax;
            else
                transaction.SellerTax += (decimal)tax;
        }

        private MonetaryOfferTax getTaxRate(MonetaryOffer offer)
        {

            return new MonetaryOfferTax()
            {
                MinimumTax = (double)(offer.MinimumTax),
                TaxRate = (double)(offer.Tax)
            };


        }

        public MonetaryOfferCost GetMonetaryOfferCost(int sellCurrencyID, int buyCurrencyID, int amount, double rate, MonetaryOfferTypeEnum offerType)
        {
            var usedCurrencyID = GetUsedCurrencyID(offerType, sellCurrencyID, buyCurrencyID);
            var currencyCountry = Persistent.Countries.FirstOrDefault(c => c.CurrencyID == usedCurrencyID);

            CountryPolicy policy = null;
            if (currencyCountry != null)
                policy = countryRepository.GetCountryPolicyById(currencyCountry.ID);
            var usedCurrency = Persistent.Currencies.GetById(usedCurrencyID);

            var offerCost = amount * (offerType == MonetaryOfferTypeEnum.Buy ? rate : 1);

            var tax = CalculateTax(offerCost, (double)(policy?.MonetaryTaxRate ?? 0), (double)(policy?.MinimumMonetaryTax ?? 0));


            return new MonetaryOfferCost()
            {
                OfferCost = offerCost,
                TaxCost = tax,
                Currency = usedCurrency
            };
        }

        private void makeMonetaryTransaction(int sellerID, int buyerID, int buyCurrencyID, int sellCurrencyID, double buyAmount, int sellAmount)
        {
            /*
            var buyCurrency = Persistent.Currencies.GetById(buyCurrencyID);
            var sellCurrency = Persistent.Currencies.GetById(sellCurrencyID);

            var buyer = entityRepository.GetById(buyerID);
            var seller = entityRepository.GetById(sellerID);

            var money = new Money()
            {
                Currency = buyCurrency,
                Amount = 
            }

            var transaction = new Transaction()
            {
                Arg1 = "Article price",
                Arg2 = string.Format("{0}({1}) paid for article in {2}({3})", payingEntity.Name, payingEntity.EntityID, newspaperEntity.Name, newspaperEntity.EntityID),
                DestinationEntityID = newspaperEntity.EntityID,
                Money = money,
                TransactionType = TransactionTypeEnum.MonetaryMarket
            };
            transactionService.MakeTransaction(transaction, useSqlTransaction: false);*/

            throw new NotImplementedException();
        }
        public double CalcualteTax(double overallPrice, MonetaryOffer offer)
        {
            return CalculateTax(overallPrice, (double)offer.Tax, (double)offer.MinimumTax);
        }
        public double CalculateTax(double overallPrice, MonetaryOfferTax tax)
        {
            return CalculateTax(overallPrice, tax.TaxRate, tax.MinimumTax);
        }
        public double CalculateTax(double overallPrice, double taxRate, double minimumTax)
        {
            return Math.Round(Math.Max(minimumTax, overallPrice * taxRate), 2);
        }
    }
}
