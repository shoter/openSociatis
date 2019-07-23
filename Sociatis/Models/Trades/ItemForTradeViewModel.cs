using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Trades
{
    public class ItemForTradeViewModel
    {
        public long TradeID { get; set; }
        public int EntityID { get; set; }
        internal DateTime DateAdded { get; set; }

        public bool CanRemove { get; set; }

        public ItemForTradeViewModel(int entityID, DateTime dateAdded, long tradeID, TradeStatusEnum tradeStatus)
        {
            EntityID = entityID;
            DateAdded = dateAdded;
            TradeID = tradeID;

            CanRemove = EntityID == SessionHelper.CurrentEntity.EntityID && tradeStatus == TradeStatusEnum.Ongoing;
        }

        public ItemForTradeViewModel(int entityID, DateTime dateAdded, long tradeID)
        {
            EntityID = entityID;
            DateAdded = dateAdded;
            TradeID = tradeID;
        }
    }
}