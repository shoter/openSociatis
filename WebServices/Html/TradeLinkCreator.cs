using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Weber.Html;

namespace WebServices.Html
{
    public class TradeLinkCreator
    {
        public static MvcHtmlString Create(Trade trade, string @class = null)
        {
            return LinkCreator.Create(
                name: $"trade #{trade.ID}",
                action: "View",
                controller: "Trade",
                routeValues: new { tradeID = trade.ID });
        }
    }
}
