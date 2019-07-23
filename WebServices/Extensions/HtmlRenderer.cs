using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WebServices.Extensions
{
    public static class HtmlRenderer
    {
        public static MvcHtmlString EntityView(this HtmlHelper helper, string entityName, int entityID)
        {
            return helper.ActionLink(entityName, "View", "Entity", new { entityID = entityID }, null);
        }
    }
}
