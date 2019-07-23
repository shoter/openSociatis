using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class TradeExtensions
    {
        public static TradeSideEnum GetTradeSide(this Trade trade, Entity entity)
        {
            return trade.GetTradeSide(entity.EntityID);
        }

        public static TradeSideEnum GetTradeSide(this Trade trade, int entityID)
        {
            if (entityID == trade.SourceEntityID)
                return TradeSideEnum.Source;
            return TradeSideEnum.Destination;
        }

        public static int? GetUsedFuel(this Trade trade, TradeSideEnum tradeSide)
        {
            if (tradeSide == TradeSideEnum.Destination)
                return trade.DestinationUsedFuelAmount;
            return trade.SourceUsedFuelAmount;
        }
        public static Entity GetTradeSide(this Trade trade, TradeSideEnum tradeSide)
        {
            if (tradeSide == TradeSideEnum.Destination)
                return trade.Destination;
            else
                return trade.Source;
        }
        public static void Set(this Trade trade, TradeStatusEnum status)
        {
            trade.TradeStatusID = (int)status;
        }
        public static bool Is(this Trade trade, TradeStatusEnum status)
        {
            return trade.TradeStatusID == (int)status;
        }
    }
}
