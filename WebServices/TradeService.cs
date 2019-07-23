using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Repository;
using Entities.Extensions;
using Common.utilities;
using WebServices.Helpers;
using Common.Extensions;
using WebServices.PathFinding;
using WebServices.structs;
using WebServices.Html;
using Common;
using System.Transactions;
using Weber.Html;
using WebServices.Companies;

namespace WebServices
{
    public class TradeService : BaseService, ITradeService
    {
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IEquipmentService equipmentService;
        private readonly ITransactionsService transactionsService;
        private readonly IWalletService walletService;
        private readonly ITradeRepository tradeRepository;
        private readonly IProductService productService;
        private readonly IRegionService regionService;
        private readonly IEmbargoRepository embargoRepository;
        private readonly IWarningService warningService;
        private readonly IWalletRepository walletRepository;
        private readonly ICompanyFinanceSummaryService companyFinanceSummaryService;

        public TradeService(IEquipmentRepository equipmentRepository, ITransactionsService transactionsService, IEquipmentService equipmentService, IWalletService walletService,
            ITradeRepository tradeRepository, IProductService productService, IRegionService regionService, IEmbargoRepository embargoRepository, IWarningService warningService,
            IWalletRepository walletRepository, ICompanyFinanceSummaryService companyFinanceSummaryService)
        {
            this.equipmentRepository = Attach(equipmentRepository);
            this.transactionsService = Attach(transactionsService);
            this.equipmentService = Attach(equipmentService);
            this.walletService = Attach(walletService);
            this.tradeRepository = Attach(tradeRepository);
            this.productService = Attach(productService);
            this.regionService = Attach(regionService);
            this.embargoRepository = Attach(embargoRepository);
            this.warningService = Attach(warningService);
            this.walletRepository = walletRepository;
            this.companyFinanceSummaryService = Attach(companyFinanceSummaryService);

        }

        public virtual void AbortTrade(Trade trade, string reason)
        {
            using (NoSaveChanges)
            {
                returnProductAndMoneyFromTrade(trade);
                trade.Set(TradeStatusEnum.Abort);

                var tradeLink = TradeLinkCreator.Create(trade);
                var msg = $"{tradeLink} was aborted due to {reason}.";
                warningService.AddWarning(trade.SourceEntityID.Value, msg);
                warningService.AddWarning(trade.DestinationEntityID.Value, msg);
            }
            tradeRepository.SaveChanges();
        }

        public virtual void AcceptTrade(Entity entity, Trade trade)
        {
            var side = trade.GetTradeSide(entity);

            if (side == TradeSideEnum.Destination)
                trade.DestinationAccepted = true;
            else
                trade.SourceAccepted = true;

            MethodResult result = CanFinishTrade(trade);
            if (result.isSuccess)
                FinishTrade(trade);
            else if (trade.DestinationAccepted && trade.SourceAccepted)
            {
                using (NoSaveChanges)
                {
                    var tradeLink = TradeLinkCreator.Create(trade);
                    var msg = $"{tradeLink} could not be finished due to : {result.Errors[0]}.";
                    warningService.AddWarning(trade.SourceEntityID.Value, msg);
                    warningService.AddWarning(trade.DestinationEntityID.Value, msg);
                    trade.DestinationAccepted = trade.SourceAccepted = false;
                }
            }

            ConditionalSaveChanges(tradeRepository);
        }

        public virtual void AddMoney(Currency currency, decimal amount, Entity entity, Trade trade)
        {
            using (NoSaveChanges)
            {
                var existing = trade.TradeMoneys.SingleOrDefault(m => m.CurrencyID == currency.ID && m.EntityID == entity.EntityID);
                trade.DestinationAccepted = trade.SourceAccepted = false;

                if (existing != null)
                    existing.Amount += amount;
                else
                    trade.TradeMoneys.Add(new TradeMoney()
                    {
                        Amount = amount,
                        CurrencyID = currency.ID,
                        EntityID = entity.EntityID,
                        TradeID = trade.ID,
                        DateAdded = DateTime.Now

                    });

                walletService.AddMoney(entity.WalletID, new Money(currency, -amount));
            }
            ConditionalSaveChanges(tradeRepository);
        }

        public virtual void AddProduct(ProductTypeEnum productType, int quality, int amount, Entity entity, Trade trade)
        {
            using (NoSaveChanges)
            {
                var existing = trade.TradeProducts.SingleOrDefault(p => p.ProductID == (int)productType && p.Quality == quality && p.EntityID == entity.EntityID);
                trade.DestinationAccepted = trade.SourceAccepted = false;
                if (existing != null)
                    existing.Amount += amount;
                else
                    trade.TradeProducts.Add(new TradeProduct()
                    {
                        Amount = amount,
                        EntityID = entity.EntityID,
                        ProductID = (int)productType,
                        TradeID = trade.ID,
                        Quality = quality,
                        DateAdded = DateTime.Now
                    });

                equipmentService.RemoveProductsFromEquipment(productType, amount, quality, entity.Equipment);
            }
            ConditionalSaveChanges(tradeRepository);
        }

        public virtual MethodResult CanAcceptTrade(Entity entity, Trade trade)
        {
            if (trade == null)
                return new MethodResult("Trade does not exist!");

            if (trade.GetTradeSide(TradeSideEnum.Destination).EntityID == entity.EntityID && trade.DestinationAccepted)
                return new MethodResult("You already accepted this trade!");

            if (trade.GetTradeSide(TradeSideEnum.Source).EntityID == entity.EntityID && trade.SourceAccepted)
                return new MethodResult("You already accepted this trade!");

            if (DoesTradeNeedPath(trade) && GetPathForTrade(trade) == null)
                return new MethodResult("There is no path between you for that!");

            if (HaveEnoughFuel(entity, trade) == false)
                return new MethodResult("You do not have enoguh fuel to accept this trade!");

            return CanHaveAccess(entity, trade);
        }

        public virtual int? GetNeededFuel(Entity entity, Trade trade)
        {
            if (trade.Source.Is(EntityTypeEnum.Citizen) || trade.Destination.Is(EntityTypeEnum.Citizen))
                return 0;

            var products = trade.TradeProducts
                .Where(prod => prod.EntityID == entity.EntityID).ToList();
            decimal fuelNeeded = 0;
            if (products.Count > 0)
            {
                var path = GetPathForTrade(trade);
                if (path == null)
                    return null;
                foreach (var product in products)
                {
                    fuelNeeded += productService.GetFuelCostForTranposrt(path, (ProductTypeEnum)product.ProductID, product.Quality, product.Amount);
                }
            }

            return (int)Math.Ceiling(fuelNeeded);

        }

        public virtual Path GetPathForTrade(Trade trade)
        {
            return GetPath(trade.Source, trade.Destination);
        }

        private Path GetPath(Entity source, Entity destination)
        {
            return regionService.GetPathBetweenRegions(source.GetCurrentRegion(), destination.GetCurrentRegion(),
                new TradeRegionSelector(source.GetCurrentCountry(), destination.GetCurrentCountry(), embargoRepository));
        }

        public virtual bool HaveEnoughFuel(Entity entity, Trade trade)
        {
            var fuelCost = GetNeededFuel(entity, trade);

            if (fuelCost.HasValue == false)
                return false;

            if (fuelCost > 0)
            {
                if (equipmentService.HaveItem(entity.Equipment, ProductTypeEnum.Fuel, 1, fuelCost.Value).IsError)
                    return false;
            }


            return true;
        }

        public virtual bool DoesTradeNeedPath(Trade trade)
        {
            return trade.TradeProducts.Count > 0 && trade.Source.Is(EntityTypeEnum.Citizen) == false;
        }


        public virtual MethodResult CanAddMoney(Currency currency, decimal amount, Entity entity, Trade trade)
        {
            if (trade.Is(TradeStatusEnum.Ongoing) == false)
                return new MethodResult("Trade is not active!");

            if (amount <= 0)
                return new MethodResult($"You must add least add 0.01 {currency.Symbol}");

            if (walletService.HaveMoney(entity.WalletID, new Money(currency, amount)) == false)
                return new MethodResult("You do not have enough money!");

            return CanHaveAccess(entity, trade);
        }

        public virtual bool WillTradeUseFuel(Trade trade, Entity entity)
        {
            if (trade.Source.Is(EntityTypeEnum.Citizen) || trade.Destination.Is(EntityTypeEnum.Citizen))
                return false;

            if (trade.TradeProducts
                .Where(prod => prod.EntityID == entity.EntityID)
                .Any())
                return true;

            return false;
        }

        public virtual MethodResult CanAddProduct(ProductTypeEnum productType, int quality, int amount, Entity entity, Trade trade)
        {
            if (trade.Is(TradeStatusEnum.Ongoing) == false)
                return new MethodResult("Trade is not active!");

            if (amount <= 0)
                return new MethodResult($"You must add least add 1 {productType.ToHumanReadable()}");

            MethodResult result = equipmentService.HaveItem(entity.Equipment, productType, quality, amount);
            if (result.IsError) return result;
            var secondSide = GetSecondSide(trade, entity);

            var possibleItems = equipmentService.GetAllowedProductsForEntity(secondSide.GetEntityType());

            if (possibleItems.Contains(productType) == false)
                return new MethodResult($"{secondSide.Name.FirstUpper()} cannot receive this product from you!");

            if (entity.Is(EntityTypeEnum.Citizen))
            {

                if (secondSide.Is(EntityTypeEnum.Citizen))
                {
                    if (secondSide.GetCurrentRegion().ID != entity.GetCurrentRegion().ID)
                        return new MethodResult("You must be in the same region with other citizen to exchange products!");
                }
                else
                    return new MethodResult("You can only trade products with citizens!");
            }
            else if (entity.Is(EntityTypeEnum.Company))
            {
                var possibleEntities = new EntityTypeEnum[]
                    {
                        EntityTypeEnum.Company,
                        EntityTypeEnum.Newspaper
                    };

                if (secondSide.Is(possibleEntities) == false)
                    return new MethodResult($"You cannot trade products with {secondSide.Name}!");

                if (secondSide.Is(EntityTypeEnum.Company))
                {
                    possibleItems = equipmentService.GetAllowedProductsForCompany(secondSide.Company);

                    if (possibleItems.Contains(productType) == false)
                        return new MethodResult($"{secondSide.Name.FirstUpper()} cannot receive this product from you!");
                }

                return MethodResult.Success;

            }
            else
                return new MethodResult("You cannot use products in trade!");

            return CanHaveAccess(entity, trade);
        }

        public virtual Entity GetSecondSide(Trade trade, Entity entity)
        {
            return trade.SourceEntityID == entity.EntityID ? trade.Destination : trade.Source;
        }

        public virtual MethodResult CanCancelTrade(Entity entity, Trade trade)
        {
            if (trade == null)
                return new MethodResult("Trade does not exist!");

            if (trade.Is(TradeStatusEnum.Ongoing) == false)
                return new MethodResult("Trade is not active!");


            return CanHaveAccess(entity, trade);
        }


        public virtual MethodResult CanHaveAccess(Entity entity, Trade trade)
        {
            if (trade == null)
                return new MethodResult("Trade does not exist!");

            if (entity.EntityID != trade.SourceEntityID && entity.EntityID != trade.DestinationEntityID)
                return new MethodResult("It's not your trade transaction!");
            return MethodResult.Success;
        }

        public virtual List<EquipmentItem> GetItemsForTrade(Entity entity, Trade trade)
        {
            var disallowed = new ProductTypeEnum[] {
                ProductTypeEnum.SellingPower,
                ProductTypeEnum.UpgradePoints,
                ProductTypeEnum.MedicalSupplies
            }.Cast<int>().ToArray<int>();

            return equipmentRepository.Where(eq => eq.ID == entity.EquipmentID)
                .SelectMany(eq => eq.EquipmentItems)
                .Where(ei => disallowed.Contains(ei.ProductID) == false)
                .ToList();
        }

        public virtual List<WalletMoney> GetMoneyForTrade(Entity entity, Trade trade)
        {
            return walletRepository.Where(w => w.ID == entity.WalletID)
                .SelectMany(w => w.WalletMoneys)
                .ToList();
        }

        public virtual void CancelTrade(Trade trade, Entity who)
        {
            using (NoSaveChanges)
            {
                returnProductAndMoneyFromTrade(trade);
                trade.Set(TradeStatusEnum.Cancel);
                if (trade.GetTradeSide(who) == TradeSideEnum.Destination)
                    trade.DestinationAccepted = false;
                else
                    trade.SourceAccepted = false;

                var entityLink = EntityLinkCreator.Create(who);
                var tradeLink = TradeLinkCreator.Create(trade);
                var msg = $"{entityLink} cancelled {tradeLink}.";
                var secondSide = GetSecondSide(trade, who);
                warningService.AddWarning(secondSide.EntityID, msg);
            }



            tradeRepository.SaveChanges();
        }

        public virtual MethodResult CanFinishTrade(Trade trade)
        {
            if (trade.DestinationAccepted == false || trade.SourceAccepted == false)
                return new MethodResult("Not everyone accepted trade");
            if (trade.Is(TradeStatusEnum.Ongoing) == false)
                return new MethodResult("You can only end active trade!");

            var sourceFuelCost = GetNeededFuel(trade.Source, trade);
            if (sourceFuelCost > 0 && equipmentService.HaveItem(trade.Source.Equipment, ProductTypeEnum.Fuel, 1, sourceFuelCost.Value).IsError)
            {
                var sourceLink = EntityLinkCreator.Create(trade.Source);
                var msg = $"{sourceLink} did not have enough fuel to complete trade";
                return new MethodResult(msg);
            }

            var destinationFuelCost = GetNeededFuel(trade.Destination, trade);
            if (destinationFuelCost > 0 && equipmentService.HaveItem(trade.Destination.Equipment, ProductTypeEnum.Fuel, 1, destinationFuelCost.Value).IsError)
            {
                var destinationLink = EntityLinkCreator.Create(trade.Destination);
                var msg = $"{destinationLink} did not have enough fuel to complete trade";
                return new MethodResult(msg);
            }

            var freeSourceSpace = equipmentService.GetUnusedInventorySpace(trade.Source.Equipment);
            var neededSourceSpace = trade.TradeProducts.Where(p => p.EntityID == trade.DestinationEntityID).Sum(p => p.Amount) - sourceFuelCost;
            if (freeSourceSpace < neededSourceSpace)
            {
                var sourceLink = EntityLinkCreator.Create(trade.Source);
                var msg = $"{sourceLink} did not have enough inventory space to complete trade";
                return new MethodResult(msg);
            }

            var freeDestinationSpace = equipmentService.GetUnusedInventorySpace(trade.Destination.Equipment);
            var neededDestinationSpace = trade.TradeProducts.Where(p => p.EntityID == trade.SourceEntityID).Sum(p => p.Amount) - destinationFuelCost;
            if (freeDestinationSpace < neededDestinationSpace)
            {
                var sourceLink = EntityLinkCreator.Create(trade.Destination);
                var msg = $"{sourceLink} did not have enough inventory space to complete trade";
                return new MethodResult(msg);
            }


            return MethodResult.Success;
        }

        public virtual Trade StartTrade(Entity source, Entity destination)
        {
            Trade trade = new Trade()
            {
                DateTime = DateTime.Now,
                Day = GameHelper.CurrentDay,
                DestinationAccepted = false,
                DestinationEntityID = destination.EntityID,
                DestinationUsedFuelAmount = null,
                SourceAccepted = false,
                SourceUsedFuelAmount = null,
                SourceEntityID = source.EntityID,
                TradeStatusID = (int)TradeStatusEnum.Ongoing,
                UpdatedDate = DateTime.Now,
            };



            tradeRepository.Add(trade);
            ConditionalSaveChanges(tradeRepository);

            var sourceLink = EntityLinkCreator.Create(source);
            var tradeLink = TradeLinkCreator.Create(trade);
            var msg = $"{sourceLink} started {tradeLink} with you.";
            warningService.AddWarning(destination.EntityID, msg);

            return trade;
        }

        public virtual MethodResult CanStartTrade(Entity source, Entity destination)
        {
            return MethodResult.Success;
        }

        public virtual void FinishTrade(Trade trade)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                trade.Set(TradeStatusEnum.Success);
                Entity destination = trade.Destination;
                Entity source = trade.Source;
                using (NoSaveChanges)
                {
                    foreach (var money in trade.TradeMoneys.ToList())
                    {
                       
                        var secondSide = GetSecondSide(trade, money.Entity);
                        walletService.AddMoney(secondSide.WalletID, new Money(money.CurrencyID, money.Amount));

                        if (secondSide.Is(EntityTypeEnum.Company))
                        {
                            var company = secondSide.Company;
                            companyFinanceSummaryService.AddFinances(company, new TradeBalanceFinance(money.Amount, money.CurrencyID));
                        }

                        if (money.Entity.Is(EntityTypeEnum.Company))
                        {
                            var company = money.Entity.Company;
                            companyFinanceSummaryService.AddFinances(company, new TradeBalanceFinance(-money.Amount, money.CurrencyID));
                        }

                    }

                    foreach (var product in trade.TradeProducts.ToList())
                    {
                        var secondSide = GetSecondSide(trade, product.Entity);
                        equipmentService.GiveItem((ProductTypeEnum)product.ProductID, product.Amount, product.Quality, secondSide.Equipment);
                    }

                    var sourceFuelCost = GetNeededFuel(trade.Source, trade);
                    var destinationFuelCost = GetNeededFuel(trade.Destination, trade);

                    trade.DestinationUsedFuelAmount = destinationFuelCost;
                    trade.SourceUsedFuelAmount = sourceFuelCost;

                    if (sourceFuelCost > 0)
                        equipmentService.RemoveProductsFromEquipment(ProductTypeEnum.Fuel, sourceFuelCost.Value, 1, trade.Source.Equipment);
                    if (destinationFuelCost > 0)
                        equipmentService.RemoveProductsFromEquipment(ProductTypeEnum.Fuel, destinationFuelCost.Value, 1, trade.Destination.Equipment);

                    var tradeLink = TradeLinkCreator.Create(trade);
                    string msg = $"{tradeLink} has been successfully finished";

                    warningService.AddWarning(destination.EntityID, msg);
                    warningService.AddWarning(source.EntityID, msg);
                }

                ConditionalSaveChanges(tradeRepository);

                trs.Complete();
            }
        }





        private void returnProductAndMoneyFromTrade(Trade trade)
        {
            returnProductFromTrade(trade);
            returnMoneyFromTrade(trade);
            ConditionalSaveChanges(equipmentRepository);
        }
        private void returnProductFromTrade(Trade trade)
        {
            using (NoSaveChanges)
            {
                var items = trade.TradeProducts.ToList();

                foreach (var item in items)
                {
                    returnTradeProductFromTrade(trade, item);
                }
            }
            ConditionalSaveChanges(equipmentRepository);
        }

        private void returnTradeProductFromTrade(Trade trade, TradeProduct item)
        {
            equipmentService.GiveItem((ProductTypeEnum)item.ProductID, item.Amount, item.Quality, item.Entity.Equipment);
            trade.TradeProducts.Remove(item);
        }

        private void returnMoneyFromTrade(Trade trade)
        {
            using (NoSaveChanges)
            {
                var moneys = trade.TradeMoneys.ToList();
                foreach (var money in moneys)
                {
                    returnTradeMoneyFromTrade(trade, money);
                }
            }

            ConditionalSaveChanges(equipmentRepository);
        }

        private void returnTradeMoneyFromTrade(Trade trade, TradeMoney money)
        {
            walletService.AddMoney(money.Entity.WalletID, new structs.Money(money.CurrencyID, money.Amount));
            trade.TradeMoneys.Remove(money);
        }

        public MethodResult CanRemoveProduct(TradeProduct product, Entity entity, Trade trade)
        {
            if (trade == null)
                return new MethodResult("Trade does not exist!");
            if (product == null)
                return new MethodResult("Product does not exist!");

            if (product.EntityID != entity.EntityID)
                return new MethodResult("It is not your product!");

            if (trade.Is(TradeStatusEnum.Ongoing) == false)
                return new MethodResult("Trade is not active!");

            return CanHaveAccess(entity, trade);
        }

        public MethodResult CanRemoveMoney(TradeMoney money, Entity entity, Trade trade)
        {
            if (trade == null)
                return new MethodResult("Trade does not exist!");
            if (money == null)
                return new MethodResult("Money does not exist!");

            if (money.EntityID != entity.EntityID)
                return new MethodResult("It is not your money!");

            if (trade.Is(TradeStatusEnum.Ongoing) == false)
                return new MethodResult("Trade is not active!");

            return CanHaveAccess(entity, trade);
        }

        public void RemoveProduct(TradeProduct product, Trade trade)
        {
            returnTradeProductFromTrade(trade, product);
            ConditionalSaveChanges(tradeRepository);
        }

        public void RemoveMoney(TradeMoney money, Trade trade)
        {
            returnTradeMoneyFromTrade(trade, money);
            ConditionalSaveChanges(tradeRepository);
        }

        public bool ShouldAbortTrade(Trade trade)
        {
            if (trade.Is(TradeStatusEnum.Ongoing))
            {
                var timeLeft = TimeHelper.CalculateDateDiffrence(
                    startDay: trade.Day,
                    currentDay: GameHelper.CurrentDay,
                    startDateTime: trade.UpdatedDate,
                    currentDateTime: DateTime.Now);

                if (Math.Abs(timeLeft.TotalHours) >= 2)
                    return true;

            }
            return false;
        }

        public void CancelInactiveTrade()
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var inactiveTrades = tradeRepository.GetTradesThatAreHoursOld(2)
                    .Where(t => t.TradeStatusID == (int)TradeStatusEnum.Ongoing)
                    .ToList();

                foreach (var trade in inactiveTrades)
                {
                    if (ShouldAbortTrade(trade))
                        AbortTrade(trade, "inactivity");
                }

                trs.Complete();
            }
        }
    }
}
