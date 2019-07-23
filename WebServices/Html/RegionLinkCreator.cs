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
    public class RegionLinkCreator
    {
        public static MvcHtmlString Create(Region region, string @class = null)
        {
            return LinkCreator.Create(region.Name, "View", "Region", new { regionID = region.ID }, @class);
        }
    }
}
