using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebServices.Html
{
    public abstract class BaseLinkCreator : ILinkCreator
    {
        public MvcHtmlString Create(string name, string action, string controller, object routeValues)
        {
            return Create(name, action, controller, routeValues, null);
        }

        public MvcHtmlString Create(string name, string action, string controller)
        {
            return Create(name, action, controller, null, null);
        }

        public abstract MvcHtmlString Create(string name, string action, string controller, object routeValues, string @class);
    }
}
