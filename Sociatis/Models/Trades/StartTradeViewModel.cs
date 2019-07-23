using Sociatis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Trades
{
    public class StartTradeViewModel
    {
        public Select2AjaxViewModel EntitySelector { get; set; }

        public StartTradeViewModel()
        {
            EntitySelector = Select2AjaxViewModel.Create<TradeController>(c => c.GetEntitiesToTrade(null), "EntityID", null, "");
        }
    }
}