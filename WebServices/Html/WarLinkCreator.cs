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
    public class WarLinkCreator
    {
        public static MvcHtmlString Create(War war, string @class = null)
        {
            string name = (war.IsRessistanceWar ? "ressistance " : "") + "war";
            return LinkCreator.Create($"{name} #{war.ID}", "view", "war", new { warID = war.ID }, @class);
        }
    }
}
