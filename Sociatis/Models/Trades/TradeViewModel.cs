using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Trades
{
    public class TradeViewModel
    {
        public long ID { get; set; }
        public bool CanSendProducts { get; set; } = true;

        public string SourceName { get; set; }
        public ImageViewModel SourceImage { get; set; }
        public bool SourceAccepted { get; set; }
        public int? SourceID { get; set; }

        public string DestinationName { get; set; }
        public int? DestinationID { get; set; }
        public ImageViewModel DestinationImage { get; set; }
        public bool DestinationAccepted { get; set; }

        public TradeStatusEnum TradeStatus { get; set; }

        public List<MoneyTradeViewModel> MoneyToTrade { get; set; }
        public List<ProductTradeViewModel> ProductToTrade { get; set; }

        public List<ItemForTradeViewModel> SourceItems { get; set; } = new List<ItemForTradeViewModel>();
        public List<ItemForTradeViewModel> DestinationItems { get; set; } = new List<ItemForTradeViewModel>();

        public bool CanAccept { get; set; }
        public bool CanCancel { get; set; }

        public TradeSideEnum TradeSide { get; set; }
        public int? FuelCost { get; set; }

        public TradeViewModel(Trade trade, IEnumerable<EquipmentItem> items, IEnumerable<WalletMoney> moneys, ITradeService tradeService)
        {
            ID = trade.ID;

            MoneyToTrade = moneys.ToList().Select(m => new MoneyTradeViewModel(m)).ToList();
            ProductToTrade = items.ToList().Select(i => new ProductTradeViewModel(i)).ToList();

            SourceName = trade.Source?.Name;
            SourceImage = new ImageViewModel(trade.Source?.ImgUrl);
            SourceID = trade.SourceEntityID;

            DestinationImage = new ImageViewModel(trade?.Destination?.ImgUrl);
            DestinationName = trade.Destination?.Name;
            DestinationID = trade.DestinationEntityID;

            SourceAccepted = trade.SourceAccepted;
            DestinationAccepted = trade.DestinationAccepted;

            TradeStatus = (TradeStatusEnum)trade.TradeStatusID;

            loadProducts(trade);
            loadMoney(trade);

            SourceItems = SourceItems.OrderBy(i => i.DateAdded).ToList();
            DestinationItems = DestinationItems.OrderBy(i => i.DateAdded).ToList();

            CanCancel = trade.Is(TradeStatusEnum.Ongoing);

            TradeSide = trade.GetTradeSide(SessionHelper.CurrentEntity);
            CanAccept = TradeStatus == TradeStatusEnum.Ongoing &&
                ((trade.SourceAccepted == false && TradeSide == TradeSideEnum.Source) || (trade.DestinationAccepted == false && TradeSide == TradeSideEnum.Destination));

                FuelCost = TradeSide == TradeSideEnum.Source ? trade.SourceUsedFuelAmount : trade.DestinationUsedFuelAmount;
            var projectedCost = tradeService.GetNeededFuel(SessionHelper.CurrentEntity, trade);
            if (projectedCost > 0)
                FuelCost = projectedCost;


        }

        private void loadMoney(Trade trade)
        {
            foreach (var money in trade.TradeMoneys.ToList())
            {
                TradeSideEnum side = trade.GetTradeSide(money.EntityID);

                if (side == TradeSideEnum.Destination)
                    DestinationItems.Add(new MoneyForTradeViewModel(money, TradeStatus));
                else
                    SourceItems.Add(new MoneyForTradeViewModel(money, TradeStatus));
            }
        }

        private void loadProducts(Trade trade)
        {
            foreach (var item in trade.TradeProducts.ToList())
            {
                TradeSideEnum side = trade.GetTradeSide(item.EntityID);

                if (side == TradeSideEnum.Destination)
                    DestinationItems.Add(new ProductForTradeViewModel(item, TradeStatus));
                else
                    SourceItems.Add(new ProductForTradeViewModel(item, TradeStatus));
            }
        }


    }
}