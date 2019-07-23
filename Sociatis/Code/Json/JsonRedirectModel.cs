using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weber.Helpers;
using Weber.Html;

namespace Sociatis.Code.Json
{
    public class JsonRedirectModel : JsonModelBase
    {
        public string Url { get; set; }
        public JsonRedirectModel(string url)
        {
            Url = url;
            Status = JsonStatusEnum.Redirect;
        }

        public JsonRedirectModel(string actionName, string controllerName, object routeValues)
            : this(HtmlHelper.UrlHelper.Action(actionName, controllerName, routeValues))
        { }
    }
}
