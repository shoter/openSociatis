using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebServices.Html
{
    public class MvcLinkCreator : BaseLinkCreator
    {
        static internal UrlHelper UrlHelper => Weber.Helpers.HtmlHelper.UrlHelper;

        public override MvcHtmlString Create(string name, string action, string controller, object routeValues, string @class)
        {
            var a = new TagBuilder("a");

            a.InnerHtml = name;
            a.Attributes["href"] = UrlHelper.Action(action, controller, routeValues);
            a.Attributes["class"] = @class;

            return MvcHtmlString.Create(a.ToString());
        }
    }
}
