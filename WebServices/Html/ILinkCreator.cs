using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebServices.Html
{
    public interface ILinkCreator
    {
        MvcHtmlString Create(string name, string action, string controller, object routeValues, string @class);
        MvcHtmlString Create(string name, string action, string controller, object routeValues);
        MvcHtmlString Create(string name, string action, string controller);

    }
}
