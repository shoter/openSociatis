using Common;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.enums.Attributes;
using Entities.Extensions;
using Entities.Groups;
using Entities.Models.Market;
using Entities.Repository;
using SociatisCommon.Errors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices.BigParams.MarketOffers;
using WebServices.Companies;
using WebServices.enums;
using WebServices.Helpers;
using WebServices.PathFinding;
using WebServices.structs;
using WebServices.structs.Market;

namespace WebServices
{
    public class MarketService : BaseService, IMarketService
    {
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IMarketOfferRepository marketOfferRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IRegionService regionService;
        private readonly IEntityRepository entityRepository;
        private readonly ITransactionsService transactionService;
        private readonly IWalletService walletService;
        private readonly IProductTaxRepository productTaxRepository;
        private readonly IProductService productService;
        private readonly IEmbargoRepository embargoRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IEquipmentService equipmentService;
        private readonly ICompanyFinanceSummaryService companyFinanceSummaryService;
        public MarketService(IEquipmentRepository equipmentRepository, IMarketOfferRepository marketOfferRepository, ICompanyRepository companyRepository,
            IRegionService regionService, IEntityRepository entityRepository, ITransactionsService transactionService, IWalletService walletService,
            IProductTaxRepository productTaxRepository, IProductService productService, IEmbargoRepository embargoRepository, ICountryRepository countryRepository,
            IEquipmentService equipmentService, ICompanyFinanceSummaryService companyFinanceSummaryService)
        {
            this.equipmentRepository = equipmentRepository;
            this.marketOfferRepository = marketOfferRepository;
            this.companyRepository = companyRepository;
            this.regionService = regionService;
            this.entityRepository = entityRepository;
            this.transactionService = transactionService;
            this.walletService = walletService;
            this.productTaxRepository = productTaxRepository;
            this.productService = productService;
            this.embargoRepository = embargoRepository;
            this.countryRepository = countryRepository;
            this.equipmentService = equipmentService;
            this.companyFinanceSummaryService = companyFinanceSummaryService;
        }

        public MarketOffer AddOffer(AddMarketOfferParameters ps)
        {

            var offer = new MarketOffer()
            {
                Amount = ps.Amount,
                CompanyID = ps.CompanyID,
                CountryID = ps.CountryID,
                ProductID = (int)ps.ProductType,
                Price = (decimal)ps.Price,
                Quality = ps.Quality
            };

            var company = companyRepository.GetById(ps.CompanyID);
            Country foreignCountry = null;
            if (ps.CountryID.HasValue)
                foreignCountry = countryRepository.GetById(ps.CountryID.Value);

            offer.CurrencyID = offer.CountryID.HasValue ?
                Persistent.Countries.GetById(foreignCountry.ID).CurrencyID :
                Persistent.Countries.GetById(company.Region.CountryID.Value).CurrencyID;

            marketOfferRepository.Add(offer);

            var equipment = equipmentRepository.First(e => e.Entities.FirstOrDefault().EntityID == ps.CompanyID);

            equipmentRepository.RemoveEquipmentItem(equipment.ID, (int)ps.ProductType, ps.Quality, ps.Amount);



            var cost = CalculateProductCost(ps.Amount, ps.Price, company.Region?.CountryID, ps.CountryID, ps.ProductType);




            MakeAddOfferTransactions(cost, company, foreignCountry);

            marketOfferRepository.SaveChanges();

            return offer;
        }

        public MethodResult CanMakeOffer(Company company, int amount, decimal price, ProductTypeEnum productType, int quality, int? countryID, List<int> embargoedCountries)
        {
            var stock = equipmentRepository.GetEquipmentItem(company.Entity.EquipmentID.Value, (int)productType, quality);

            if (stock == null || stock.Amount < amount)
                return new MethodResult("There is not enough material to create this market offer!");

            if (countryID.HasValue && embargoedCountries.Contains(countryID.Value))
                return new MethodResult("There is embargo on this country!");

            if (countryID > 0)
            {
                var country = Persistent.Countries.FirstOrDefault(c => c.ID == countryID);

                if (country == null)
                    return new MethodResult("Country does not exist!");

                var marketCurrency = country.Currency;


                var countryPolicy = countryRepository.GetCountryPolicyById(country.ID);

                var cost = CalculateProductCost(amount, price, company.Region?.CountryID, countryID, productType);

                var money = walletService.GetWalletMoney(company.Entity.WalletID, marketCurrency.ID);

                decimal marketCost = countryPolicy.MarketOfferCost + (cost.ExportTax ?? 0m);

                if (money.Amount < marketCost)
                    return new MethodResult($"You do not have {marketCost} {marketCurrency.ShortName} to post this offer");

                if (cost.ImportTax > 0)
                {
                    decimal companyCountryCost = (decimal)cost.ImportTax;
                    var localCurrency = Persistent.Countries.GetById(company.Region.CountryID.Value).Currency;

                    if (companyCountryCost > 0.0m)
                    {
                        money = walletService.GetWalletMoney(company.Entity.WalletID, localCurrency.ID);
                        if (money.Amount < companyCountryCost)
                            return new MethodResult($"You do not have {marketCost} {localCurrency.ShortName} to post this offer");
                    }
                }

            }

            return MethodResult.Success;
        }

        public MethodResult RemoveOffer(int offerID)
        {
            var offer = marketOfferRepository.GetById(offerID);
            using (NoSaveChanges)
                equipmentService.GiveItem((ProductTypeEnum)offer.ProductID, offer.Amount, offer.Quality, offer.Company.Entity.Equipment);

            marketOfferRepository.Remove(offerID);
            marketOfferRepository.SaveChanges();

            return MethodResult.Success;
        }

        public MethodResult Buy(int offerID, int buyerID, int amount)
        {
            var offer = marketOfferRepository.GetById(offerID);
            var buyer = entityRepository.GetById(buyerID);

            return Buy(offer, buyer, amount,buyer.WalletID);
        }

        public MethodResult Buy(MarketOffer offer, Entity buyer, int amount)
        {
            return Buy(offer, buyer, amount, buyer.WalletID);
        }



        public MethodResult Buy(MarketOffer offer, Entity buyer, int amount, int walletID)
        {
            using (var scope = transactionScopeProvider.CreateTransactionScope())
            {
                var eqID = buyer.EquipmentID.Value;
                var sellerEntity = offer.Company.Entity;
                var localCountry = sellerEntity.GetCurrentCountry();

                if (offer.GetProductType() == ProductTypeEnum.House)
                {
                    var houseService = DependencyResolver.Current.GetService<IHouseService>();
                    houseService.CreateHouseForCitizen(buyer.Citizen);
                }
                else
                    equipmentRepository.AddEquipmentItem(eqID, offer.ProductID, offer.Quality, amount);



                var cost = GetOfferCost(offer, buyer, amount);

                if (sellerEntity.Company.CompanyTypeID == (int)CompanyTypeEnum.Shop)
                    equipmentRepository.RemoveEquipmentItem(sellerEntity.EquipmentID.Value, (int)ProductTypeEnum.SellingPower, 1, amount);

                if (buyer.EntityTypeID == (int)EntityTypeEnum.Company)
                    UseFuelInBuyProcess(eqID, cost);

                var currency = Persistent.Currencies.First(c => c.ID == offer.CurrencyID);

                if (makeCompanyTransactionForBuy(offer, buyer, sellerEntity, cost, currency, amount, walletID) != TransactionResult.Success)
                    return MethodResult.Failure;

                if (makeLocalCountryTransactionForBuy(offer, buyer, sellerEntity, localCountry, cost, currency, amount, walletID) != TransactionResult.Success)
                    return MethodResult.Failure;

                //there is no vat because company is not paying vat (directly). Citizen is paying the vat.
                ICompanyFinance[] finances = new ICompanyFinance[] {
                    new SellRevenueFinance(cost.BasePrice,  currency.ID),
                };

                companyFinanceSummaryService.AddFinances(offer.Company, finances);

                /*if (cost.ExportTax > 0)
                {
                    makeExportTransactionForBuy(offer, buyer, sellerEntity, cost, amount);
                }*/

                offer.Amount -= amount;
                if (offer.Amount <= 0)
                {
                    marketOfferRepository.Remove(offer.ID);
                }

                equipmentRepository.SaveChanges();
                scope.Complete();

                return MethodResult.Success;
            }
        }

        private TransactionResult makeExportTransactionForBuy(MarketOffer offer, Entity buyer, Entity sellerEntity, OfferCost cost, int amount, int walletID)
        {
            var exportCountry = buyer.GetCurrentCountry();

            var exportTransaction = new Transaction()
            {
                Arg1 = "Export Tax Cost - Market",
                Arg2 = string.Format("{0}({1}) bought {2} {3} from {4}({5})", buyer.Name, buyer.EntityID, amount,
                ((ProductTypeEnum)offer.ProductID).ToHumanReadable(), sellerEntity.Name, sellerEntity.EntityID),
                DestinationEntityID = exportCountry.ID,
                Money = cost.ExportMoney,
                SourceEntityID = buyer.EntityID,
                TransactionType = TransactionTypeEnum.MarketOfferCost,
                SourceWalletID = walletID
            };

            return transactionService.MakeTransaction(exportTransaction);
        }

        private TransactionResult makeCompanyTransactionForBuy(MarketOffer offer, Entity buyer, Entity sellerEntity, OfferCost cost, Currency currency, int amount, int walletID)
        {
            var companyMoney = new Money()
            {
                Amount = (decimal)(cost.BasePrice),
                Currency = currency
            };


            var companyTransaction = new Transaction()
            {
                Arg1 = "Company Cost - Market",
                Arg2 = string.Format("{0}({1}) bought {2} {3} from {4}({5})", buyer.Name, buyer.EntityID, amount,
                    ((ProductTypeEnum)offer.ProductID).ToHumanReadable(), sellerEntity.Name, sellerEntity.EntityID),
                DestinationEntityID = sellerEntity.EntityID,
                Money = companyMoney,
                SourceEntityID = buyer.EntityID,
                TransactionType = TransactionTypeEnum.MarketOfferCost,
                SourceWalletID = walletID
            };

            return transactionService.MakeTransaction(companyTransaction);
        }

        private TransactionResult makeLocalCountryTransactionForBuy(MarketOffer offer, Entity buyer, Entity sellerEntity, Country localCountry, OfferCost cost, Currency currency, int amount, int walletID)
        {
            if (cost.VatCost.HasValue == false || cost.VatCost == 0)
                return TransactionResult.Success;

            var localCountryMoney = new Money()
            {
                Amount = (decimal)(cost.VatCost),
                Currency = currency
            };

            var localCountryTransaction = new Transaction()
            {
                Arg1 = "Local Country Cost - Market",
                Arg2 = string.Format("{0}({1}) bought {2} {3} from {4}({5})", buyer.Name, buyer.EntityID, amount,
                    ((ProductTypeEnum)offer.ProductID).ToHumanReadable(), sellerEntity.Name, sellerEntity.EntityID),
                DestinationEntityID = localCountry.ID,
                Money = localCountryMoney,
                SourceEntityID = sellerEntity.EntityID,
                TransactionType = TransactionTypeEnum.MarketOfferCost,
                SourceWalletID = walletID
            };

            return transactionService.MakeTransaction(localCountryTransaction);
        }

        private void UseFuelInBuyProcess(int eqID, OfferCost cost)
        {
            equipmentRepository.RemoveEquipmentItem(eqID, (int)ProductTypeEnum.Fuel, 1, cost.IntegerRealCost);
        }

        public MethodResult CanBuyOffer(MarketOffer offer, int amount, Entity entity)
        {
            return CanBuyOffer(offer, amount, entity, entity.WalletID);
        }

        public MethodResult CanBuyOffer(MarketOffer offer, int amount, Entity entity, int walletID)
        {
            if (offer == null)
                return new MethodResult(MarketOfferErrors.OfferNotExist);

            var productType = ((ProductTypeEnum)offer.ProductID);

            if (offer.Amount < amount)
                return new MethodResult(MarketOfferErrors.NotEnoughProducts);

            if (amount <= 0)
                return new MethodResult("Are you crazy?");

            if (amount > 1 && offer.GetProductType() == ProductTypeEnum.House)
                return new MethodResult("You cannot buy more than 1 house at same moment!");

            if (equipmentService.IsAllowedItemFor(entity, productType) == false)
                return new MethodResult(MarketOfferErrors.NotAllowedProduct);

            var result = CanBuy(offer, entity, offer.Company.Entity);
            if (result.IsError) return result;


            var cost = GetOfferCost(offer, entity, amount);

            if (cost == null)
                return new MethodResult("You cannot buy that offer!");

            if (walletService.HaveMoney(walletID, cost.ClientPriceMoney) == false)
                return new MethodResult(MarketOfferErrors.NotEnoughMoney);

            if (offer.Company.CompanyTypeID == (int)CompanyTypeEnum.Shop && (equipmentRepository.GetEquipmentItem(offer.Company.Entity.EquipmentID.Value, (int)ProductTypeEnum.SellingPower, 1)?.Amount ?? 0) < amount)
                return new MethodResult("Shop does not have enough selling power!");

            if (entity.EntityTypeID != (int)EntityTypeEnum.Citizen && cost.FuelCost > 0)
            {
                var fuel = equipmentRepository.GetEquipmentItem(entity.EquipmentID.Value, (int)ProductTypeEnum.Fuel, 1);

                if (IsEnoughFuelForTrade(fuel?.Amount, cost.IntegerRealCost, offer) == false)
                    return new MethodResult(MarketOfferErrors.NotEngouhFuel);
            }


            var equipment = entity.Equipment;

            if (equipment.CanAddNewItem(amount) == false)
            {
                return new MethodResult("You do not have enough space in inventory");
            }

            if (offer.GetProductType() == ProductTypeEnum.House)
            {
                var houseService = DependencyResolver.Current.GetService<IHouseService>();
                if (houseService.CanCreateHouseForCitizen(entity.Citizen).IsError)
                    return new MethodResult("You already have house in this region!");
            }



            return MethodResult.Success;
        }

        public virtual bool IsEnoughFuelForTrade(int? fuelInInventory, int neededFuel, MarketOffer offer)
        {
            fuelInInventory = fuelInInventory ?? 0;
            if (neededFuel > 0 == false)
                return true;

            if (offer.GetProductType() == ProductTypeEnum.Fuel)
                neededFuel -= offer.Amount;

            return fuelInInventory >= neededFuel;
        }


        public ProductCost CalculateProductCost(int amount, decimal nettoPrice, int? homeCountryID, int? sellingCountryID, ProductTypeEnum productType)
        {
            nettoPrice = Math.Round(nettoPrice, 2, MidpointRounding.AwayFromZero);
            ProductCost cost = new ProductCost() { BasePrice = amount * nettoPrice };
            var productTaxes = productService.GetAllTaxesForProduct((int)productType, homeCountryID, sellingCountryID);

            if (homeCountryID != null || sellingCountryID != null)
                cost.VatCost = Math.Round(cost.BasePrice * productTaxes.VAT, 2, MidpointRounding.AwayFromZero);
            if (sellingCountryID != null && sellingCountryID != homeCountryID)
            {
                cost.ImportTax = Math.Round(cost.BasePrice * productTaxes.ImportTax, 2, MidpointRounding.AwayFromZero);
                if (homeCountryID != null)
                    cost.ExportTax = Math.Round(cost.BasePrice * productTaxes.ExportTax, 2, MidpointRounding.AwayFromZero);
            }

            return cost;
        }

        public OfferCost GetOfferCost(MarketOffer offer, Entity buyer, int amount)
        {
            return GetOfferCost(offer, amount, buyer.GetCurrentRegion(), CanUseFuel(buyer));
        }

        public OfferCost GetOfferCost(MarketOffer offer, int amount, Region destinationRegion, bool useFuel)
        {
            var destinationCountry = destinationRegion.Country;

            OfferCost cost = new OfferCost();
            var sellerRegion = offer.Company.Entity.GetCurrentRegion();


            if (useFuel)
            {
                var buyerRegion = destinationRegion;
                var path = regionService.GetPathBetweenRegions(buyerRegion, sellerRegion, new TradeRegionSelector(offer.Company.Region.Country, destinationCountry, embargoRepository));

                if (path == null)
                    return null;

                cost.FuelCost = GetFuelCostForOffer(offer, path, amount);
            }

            cost.Set(CalculateProductCost(amount, offer.Price, offer.Company.Region.CountryID, destinationCountry.ID, (ProductTypeEnum)offer.ProductID));
            cost.CurrencyID = offer.CurrencyID;



            cost.FuelCost = Math.Round(cost.FuelCost, 2);

            return cost;

        }

        public OfferCost GetOfferCost(MarketOfferModel offer, Entity buyer, int amount)
        {
            OfferCost cost = new OfferCost();
            var sellerRegion = Persistent.Regions.GetById(offer.CompanyRegionID);
            var sellerCountry = Persistent.Countries.GetById(offer.CompanyCountryID);

            if (CanUseFuel(buyer))
            {
                var buyerRegion = buyer.GetCurrentRegion();
                var path = regionService.GetPathBetweenRegions(buyerRegion, sellerRegion, new TradeRegionSelector(sellerCountry, buyer.GetCurrentCountry(), embargoRepository));

                if (path == null)
                    return null;

                cost.FuelCost = GetFuelCostForOffer(offer, path, amount);
            }

            cost.Set(CalculateProductCost(amount, offer.Price, offer.CompanyCountryID, buyer.GetCurrentCountry().ID, (ProductTypeEnum)offer.ProductID));
            cost.CurrencyID = offer.CurrencyID;



            cost.FuelCost = Math.Round(cost.FuelCost, 2);

            return cost;

        }

        public decimal? GetFuelCostForOffer(MarketOffer offer, Entity buyer)
        {
            var entityType = (EntityTypeEnum)buyer.EntityTypeID;
            if (entityType == EntityTypeEnum.Citizen || entityType == EntityTypeEnum.Newspaper)
                return null;

            var buyerRegion = buyer.GetCurrentRegion();
            var path = regionService.GetPathBetweenRegions(buyerRegion, offer.Company.Region, new TradeRegionSelector(offer.Company.Region.Country, buyer.GetCurrentCountry(), embargoRepository));

            if (path == null)
                return null;

            return GetFuelCostForOffer(offer, path, 1);
        }

        public decimal? GetFuelCostForOffer(int marketOfferID, Region destinationRegion)
        {
            var offer = marketOfferRepository.GetById(marketOfferID);
            var buyerRegion = destinationRegion;
            var path = regionService.GetPathBetweenRegions(buyerRegion, offer.Company.Region, new TradeRegionSelector(offer.Company.Region.Country, destinationRegion.Country, embargoRepository));

            if (path == null)
                return null;

            return GetFuelCostForOffer(offer, path, 1);
        }

        public decimal? GetFuelCostForOffer(MarketOfferModel offer, Entity buyer)
        {
            var entityType = (EntityTypeEnum)buyer.EntityTypeID;
            if (entityType == EntityTypeEnum.Citizen || entityType == EntityTypeEnum.Newspaper)
                return null;

            var buyerRegion = buyer.GetCurrentRegion();

            var companyRegion = Persistent.Regions.GetById(offer.CompanyRegionID);
            var companyCountry = Persistent.Countries.GetById(offer.CompanyCountryID);

            var path = regionService.GetPathBetweenRegions(buyerRegion, companyRegion, new TradeRegionSelector(companyCountry, buyer.GetCurrentCountry(), embargoRepository));

            if (path == null)
                return null;

            return GetFuelCostForOffer(offer, path, 1);
        }

        private decimal GetFuelCostForOffer(MarketOffer offer, Path path, int amount)
        {
            return productService.GetFuelCostForTranposrt(path, (ProductTypeEnum)offer.ProductID, offer.Quality, amount);
        }

        private decimal GetFuelCostForOffer(MarketOfferModel offer, Path path, int amount)
        {
            return productService.GetFuelCostForTranposrt(path, (ProductTypeEnum)offer.ProductID, offer.Quality, amount);
        }

        public bool CanUseFuel(Entity entity)
        {
            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                case EntityTypeEnum.Country:
                case EntityTypeEnum.Newspaper:
                case EntityTypeEnum.Party:
                    return false;
            }
            return true;
        }





        public MethodResult HaveEnoughMoneyToBuy(MarketOffer offer, Entity buyer, int amount)
        {
            MethodResult result = MethodResult.Success;

            var cost = GetOfferCost(offer, buyer, amount);

            var localMoney = walletService.GetWalletMoney(buyer.WalletID, offer.CurrencyID);

            if (localMoney.Amount < cost.BasePrice + cost.ImportTax + cost.VatCost)
            {
                result.AddError("You do not have enough money!");
            }

            if (cost.ExportTax > 0)
            {
                var exportMoney = walletService.GetWalletMoney(buyer.WalletID, cost.ExportCurrencyID);
                if (exportMoney.Amount < cost.ExportTax)
                {
                    result.AddError("You do not have enough money!");
                }
            }

            return result;
        }

        public MethodResult CanBuy(int offerID, Entity buyer, Entity seller)
        {
            return CanBuy(marketOfferRepository.GetById(offerID), buyer, seller);
        }

        public MethodResult CanBuy(MarketOffer offer, Entity buyer, Entity seller)
        {
            if (buyer.EntityID == seller.EntityID)
                return new MethodResult("You cannot buy from yourself!");

            var traderFactory = new TraderFactory(equipmentService);

            var buyerTrader = traderFactory.GetTrader(buyer);
            var sellerTrader = traderFactory.GetTrader(seller);

            return buyerTrader.CanBuy(offer, sellerTrader);
        }



      /*  public MethodResult CanBuy(int? countryID, int productID, Entity buyer, Entity seller)
        {
           if(buyer.EntityID == seller.EntityID)





            /*var buyerEntityType = (EntityTypeEnum)buyer.EntityTypeID;

            CompanyTypeEnum? buyerCompanyType = (CompanyTypeEnum?)buyer?.Company?.CompanyTypeID;

            if (buyer.EntityID == seller.EntityID)
                return false;


            if (buyerEntityType == EntityTypeEnum.Citizen)
            {
                //Citizens can only buy in their region if offer is not on the country market.
                if (countryID.HasValue == false && buyer.GetCurrentRegion().ID != seller.GetCurrentRegion().ID)
                    return false;
                //citizens cannot buy things from markets where they are not present
                if (countryID != buyer.GetCurrentRegion().CountryID)
                    return false;
            }


            bool citizenAbleToBuy = true;
            bool companyAbleToBuy = true;
            bool isBuyerAShop = buyer?.Company?.CompanyTypeID == (int)CompanyTypeEnum.Shop;

            CompanyTypeEnum companyType = (CompanyTypeEnum)seller.Company.CompanyTypeID;

            switch (companyType)
            {
                case CompanyTypeEnum.Producer:
                    {
                        citizenAbleToBuy = false;
                        if (isBuyerAShop)
                            return false;
                        break;
                    }
                case CompanyTypeEnum.Manufacturer:
                    {
                        citizenAbleToBuy = false;
                        break;
                    }
                case CompanyTypeEnum.Shop:
                    {
                        citizenAbleToBuy = true;
                        if (isBuyerAShop)
                            return false;
                        break;
                    }
            }


            switch ((ProductTypeEnum)productID)
            {
                case ProductTypeEnum.Bread:
                case ProductTypeEnum.Weapon:
                case ProductTypeEnum.Tea:
                case ProductTypeEnum.MovingTicket:
                case ProductTypeEnum.House:
                    return (buyerEntityType == EntityTypeEnum.Citizen && citizenAbleToBuy) || buyerEntityType == EntityTypeEnum.Organisation
                        || isBuyerAShop;

                case ProductTypeEnum.Paper:
                    return isBuyerAShop || buyerEntityType == EntityTypeEnum.Newspaper;

                case ProductTypeEnum.Iron:
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.Fuel:
                case ProductTypeEnum.Wood:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.MedicalSupplies:
                    return buyerEntityType == EntityTypeEnum.Organisation || (buyerEntityType == EntityTypeEnum.Company && companyAbleToBuy);
                case ProductTypeEnum.UpgradePoints:
                case ProductTypeEnum.ConstructionMaterials:
                    return buyerEntityType == EntityTypeEnum.Company;

                case ProductTypeEnum.SellingPower:
                    throw new ArgumentException("Selling power is not tradeable");
                case ProductTypeEnum.Development:
                    throw new ArgumentException("Development is not tradeable");
            }

            throw new NotImplementedException();
        }*/

        public decimal GetVatForProduct(int? countryID, int productID)
        {
            if (countryID == 0)
                return 0m;

            return (decimal?)productTaxRepository
                .FirstOrDefault(t => t.CountryID == countryID && t.ProductTaxTypeID == (int)ProductTaxTypeEnum.VAT && t.ProductID == productID)?.TaxRate ?? Constants.DefaultVat;
        }

        public void MakeAddOfferTransactions(ProductCost cost, Company company, Country foreignCountry)
        {
            var companyEntity = company.Entity;
            var foreignEntity = foreignCountry?.Entity;
            var homeCountry = company.Region?.Country;
            var homeEntity = homeCountry?.Entity;

            Currency homeCurrency = null;
            Currency foreignCurrency = null;

            if (homeCountry != null)
            {
                homeCurrency = Persistent.Countries.GetCountryCurrency(homeCountry);
            }

            if (foreignCountry != null)
            {
                foreignCurrency = Persistent.Countries.GetCountryCurrency(foreignCountry);
            }

            if (homeCurrency != null && cost.ExportTax > 0 && homeCountry != null)
            {
                var money = new Money()
                {
                    Amount = cost.ExportTax.Value,
                    Currency = homeCurrency
                };

                var transaction = new Transaction()
                {
                    Arg1 = "Market offer creation - home cost",
                    Arg2 = string.Format("{0}({1}) paid for creating market offer to {2}({3})", companyEntity.Name, companyEntity.EntityID, homeEntity?.Name, homeEntity?.EntityID),
                    DestinationEntityID = homeCountry.ID,
                    Money = money,
                    SourceEntityID = company.ID,
                    TransactionType = TransactionTypeEnum.MarketOfferImportCost
                };

                companyFinanceSummaryService.AddFinances(company, new ExportTaxFinance(cost.ExportTax.Value, money.Currency.ID));

                transactionService.MakeTransaction(transaction);
            }

            if (foreignCurrency != null && cost.TotalForeignCost > 0 && foreignEntity != null)
            {
                var money = new Money()
                {
                    Amount = cost.TotalForeignCost,
                    Currency = foreignCurrency
                };

                var transaction = new Transaction()
                {
                    Arg1 = "Market offer creation - foreign cost",
                    Arg2 = string.Format("{0}({1}) paid for creating market offer to {2}({3})", companyEntity.Name, companyEntity.EntityID, foreignEntity.Name, foreignEntity.EntityID),
                    DestinationEntityID = foreignCountry.ID,
                    Money = money,
                    SourceEntityID = company.ID,
                    TransactionType = TransactionTypeEnum.MarketOfferCost
                };

                companyFinanceSummaryService.AddFinances(company, new ImportTaxFinance(cost.TotalForeignCost, money.Currency.ID));

                transactionService.MakeTransaction(transaction);
            }
        }
    }
}
