using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.Helpers;

namespace Sociatis.Code.Attributes
{
    public class DayChangeRedirect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(GameHelper.IsDayChange)
                filterContext.Result = new RedirectResult("/Special/DayChange.cshtml");
        }
    }
}