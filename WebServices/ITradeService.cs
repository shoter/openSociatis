using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ITradeService
    {
        MethodResult CanStartTrade(Entity source, Entity destination);
        MethodResult CanAcceptTrade(Entity entity, Trade trade);
        MethodResult CanFinishTrade(Trade trade);
        MethodResult CanCancelTrade(Entity entity, Trade trade);
        MethodResult CanHaveAccess(Entity entity, Trade trade);
        bool ShouldAbortTrade(Trade trade);
        MethodResult CanAddProduct(ProductTypeEnum productType, int quality, int amount, Entity entity, Trade trade);
        MethodResult CanAddMoney(Currency currency, decimal amount, Entity entity, Trade trade);

        Trade StartTrade(Entity source, Entity destination);

        void AddProduct(ProductTypeEnum productType, int quality, int amount, Entity entity, Trade trade);
        void AddMoney(Currency currency, decimal amount, Entity entity, Trade trade);

        void CancelTrade(Trade trade, Entity whoCancelled);
        void AbortTrade(Trade trade, string reason);
        void AcceptTrade(Entity entity, Trade trade);
        void FinishTrade(Trade trade);

        List<EquipmentItem> GetItemsForTrade(Entity entity, Trade trade);
        List<WalletMoney> GetMoneyForTrade(Entity entity, Trade trade);

        int? GetNeededFuel(Entity entity, Trade trade);


        MethodResult CanRemoveProduct(TradeProduct product, Entity entity, Trade trade);
        MethodResult CanRemoveMoney(TradeMoney money, Entity entity, Trade trade);

        void RemoveProduct(TradeProduct product, Trade trade);
        void RemoveMoney(TradeMoney money, Trade trade);

        void CancelInactiveTrade();
    }
}
