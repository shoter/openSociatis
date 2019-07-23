using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Common.Operations;
using WebServices.structs;
using Entities.enums;
using Entities.Extensions;
using WebServices.PathFinding;
using Entities.Repository;
using Common;
using System.Transactions;
using WebServices.Helpers;
using Weber.Html;
using WebServices.Companies;

namespace WebServices
{
    public class GiftService : BaseService, IGiftService
    {
        private readonly IWalletService walletService;
        private readonly IEquipmentService equipmentService;
        private readonly IProductService productService;
        private readonly IRegionService regionService;
        private readonly IEmbargoRepository embargoRepository;
        private readonly ITransactionsService transactionsService;
        private readonly IGiftTransactionRepository giftTransactionRepository;
        private readonly IWarningService warningService;
        private readonly ICompanyFinanceSummaryService companyFinanceSummaryService;


        public GiftService(IWalletService walletService, IEquipmentService equipmentService, IProductService productService, IRegionService regionService,
            IEmbargoRepository embargoRepository, ITransactionsService transactionsService, IGiftTransactionRepository giftTransactionRepository,
            IWarningService warningService, ICompanyFinanceSummaryService companyFinanceSummaryService)
        {
            this.walletService = walletService;
            this.equipmentService = equipmentService;
            this.productService = productService;
            this.regionService = regionService;
            this.embargoRepository = embargoRepository;
            this.giftTransactionRepository = giftTransactionRepository;
            this.transactionsService = transactionsService;
            this.warningService = warningService;
            this.companyFinanceSummaryService = companyFinanceSummaryService;
        }
        public MethodResult CanSendMoneyGift(Entity source, Entity destination, Currency currency, decimal amount)
        {
            if (source == null)
                return new MethodResult("Source not defined!");
            if (destination == null)
                return new MethodResult("Destination not defined!");
            if (source.EntityID == destination.EntityID)
                return new MethodResult("You cannot send gift to yourself!");
            if (currency == null)
                return new MethodResult("Currency not found!");
            if (amount <= 0)
                return new MethodResult($"You must send at least 0.01 {currency.Symbol}!");
            if (source.GetEntityType() == EntityTypeEnum.Country)
                return new MethodResult("You cannot make money gifts!");

            var money = new Money(currency, amount);

            if (walletService.HaveMoney(source.WalletID, money) == false)
                return new MethodResult("You do not have enough money!");



            return MethodResult.Success;
        }

        public int GetNeededFuelToSendGift(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount)
        {
            var path = regionService.GetPathBetweenRegions(source.GetCurrentRegion(), destination.GetCurrentRegion(), new TradeRegionSelector(source.GetCurrentCountry(), destination.GetCurrentCountry(), embargoRepository));
            var fuelCost = productService.GetFuelCostForTranposrt(path, productType, quality, amount);

            return (int)Math.Ceiling(fuelCost);
        }

        public bool WillGiftUseFuel(Entity source, Entity destination)
        {
            var entityTypesUsingFuel = new EntityTypeEnum[]
                {
                     EntityTypeEnum.Newspaper,
                     EntityTypeEnum.Company
                };

            if (source.Is(EntityTypeEnum.Company) && entityTypesUsingFuel.Contains(destination.GetEntityType()))
                return true;

            return false;
        }

        public MethodResult CanReceiveProductGifts(Entity source, Entity destination)
        {
            if (source == null)
                return new MethodResult("Giver does not exist!");
            if (destination == null)
                return new MethodResult("You are not giving this gift to anyone!");
            if (source.EntityID == destination.EntityID)
                return new MethodResult("You cannot send gifts to yourself!");

            switch (source.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    if (destination.Is(EntityTypeEnum.Citizen))
                    {
                        if (destination.GetCurrentRegion().ID == source.GetCurrentRegion().ID)
                            return MethodResult.Success;
                        return new MethodResult("You must be in the same region to send product gift!");
                    }
                    return new MethodResult("You can only send product gifts to citizens!");
                case EntityTypeEnum.Company:
                    if(destination.Is(EntityTypeEnum.Company) || destination.Is(EntityTypeEnum.Newspaper))
                        return MethodResult.Success;
                    return new MethodResult($"You cannot send products to {destination.Name}!");

            }

            return new MethodResult($"You cannot send product gifts to {destination.Name}");
        }

        public MethodResult CanSendProductGift(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount)
        {
            if (source == null)
                return new MethodResult("Source not defined!");
            if (destination == null)
                return new MethodResult("Destination not defined!");
            if (amount <= 0)
                return new MethodResult($"You must send at least 1 item!");
            if (source.EntityID == destination.EntityID)
                return new MethodResult("You cannot send gift to yourself!");

            MethodResult result = equipmentService.HaveItem(source.Equipment, productType, quality, amount);
            if (result.IsError)
                return result;

            var unusedSpace = equipmentService.GetUnusedInventorySpace(destination.Equipment);

            if (unusedSpace < amount)
                return new MethodResult($"{destination.Name} does not have enough space!");

            result = CanReceiveProductGifts(source, destination);
            if (result.IsError)
                return result;


            switch (source.GetEntityType())
            {
                case EntityTypeEnum.Company:
                    var possibleEntities = new EntityTypeEnum[]
                        {
                            EntityTypeEnum.Company,
                            EntityTypeEnum.Newspaper
                        };

                    var allowedProducts = equipmentService.GetAllowedProductsForEntity(destination.GetEntityType());
                    if (allowedProducts.Contains(productType) == false)
                        return new MethodResult($"You cannot send that to {destination.Name}");

                    if (destination.Is(EntityTypeEnum.Company))
                    {
                        allowedProducts = equipmentService.GetAllowedProductsForCompany(destination.Company);

                        if (allowedProducts.Contains(productType) == false)
                            return new MethodResult($"You cannot send that to {destination.Name}");
                    }

                    

                    var fuelNeeded = GetNeededFuelToSendGift(source, destination, productType, quality, amount);
                    if (equipmentService.HaveItem(source.Equipment, ProductTypeEnum.Fuel, 1, fuelNeeded).IsError)
                        return new MethodResult("You do not have enough fuel!");

                    break;
            }

            return MethodResult.Success;
        }

        public void SendMoneyGift(Entity source, Entity destination, Currency currency, decimal amount)
        {
            var money = new Money(currency, amount);
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                transactionsService.MakeGift(source, destination, money);
                var sourceLink = EntityLinkCreator.Create(source);
                string msg = $"You received {money.Amount} {currency.Symbol} from {sourceLink}.";
                warningService.AddWarning(destination.EntityID, msg);

                if (destination.Is(EntityTypeEnum.Company))
                {
                    var company = destination.Company;
                    companyFinanceSummaryService.AddFinances(company, new GiftBalanceFinance(amount, currency.ID));
                }

                if (source.Is(EntityTypeEnum.Company))
                {
                    var company = source.Company;
                    companyFinanceSummaryService.AddFinances(company, new GiftBalanceFinance(-amount, currency.ID));
                }

                trs.Complete();
            }
        }

        public void SendProductGift(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                using (NoSaveChanges)
                {
                    int? fuelUsed = null;
                    if (WillGiftUseFuel(source, destination))
                    {
                        int fuelNeeded = GetNeededFuelToSendGift(source, destination, productType, quality, amount);
                        fuelUsed = fuelNeeded;
                        equipmentService.RemoveProductsFromEquipment(ProductTypeEnum.Fuel, fuelNeeded, 1, source.Equipment);
                    }
                    equipmentService.RemoveProductsFromEquipment(productType, amount, quality, source.Equipment);
                    equipmentService.GiveItem(productType, amount, quality, destination.Equipment);
                    createGiftTransaction(source, destination, productType, quality, amount, fuelUsed);

                    var sourceLink = EntityLinkCreator.Create(source);
                    string msg = $"You received {amount} {productType.ToHumanReadable()} from {sourceLink}.";
                    warningService.AddWarning(destination.EntityID, msg);
                }
                giftTransactionRepository.SaveChanges();
                trs.Complete();
            }
        }

        private void createGiftTransaction(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount, int? fuelUsed)
        {
            var transaction = new GiftTransaction()
            {
                Amount = amount,
                Day = GameHelper.CurrentDay,
                Destination = destination,
                Source = source,
                ProductID = (int)productType,
                Quality = quality,
                Time = DateTime.Now,
                FuelUsed = fuelUsed
            };
        
            giftTransactionRepository.Add(transaction);
            ConditionalSaveChanges(giftTransactionRepository);
        }
    }
}
