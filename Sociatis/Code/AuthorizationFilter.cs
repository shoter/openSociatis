using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Annotations;
using Sociatis.Helpers;

namespace Sociatis.Code
{
    public class AuthorizationFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize([NotNull] DashboardContext context)
        {
            return SessionHelper.CurrentEntity?.Name?.ToLower() == "admin";
        }
    }
}