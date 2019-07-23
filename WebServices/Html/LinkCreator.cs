using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.Html;

namespace Weber.Html
{
    public class LinkCreator
    {
        public static ILinkCreator Current { get; set; }

        public static MvcHtmlString Create(string name, string action, string controller, object routeValues, string @class)
        {
            var creator = Current;
            if (creator == null)
                creator = new MvcLinkCreator();
            
            return creator.Create(name, action, controller, routeValues, @class);
        }

        public static MvcHtmlString Create(string name, string action, string controller, object routeValues)
        {
            return Create(name, action, controller, routeValues, null);
        }

        public static MvcHtmlString Create(string name, string action, string controller)
        {
            return Create(name, action, controller, null, null);
        }
    }
}