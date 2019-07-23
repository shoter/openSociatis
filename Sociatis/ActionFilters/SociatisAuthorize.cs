using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sociatis.ActionFilters
{
    public class SociatisAuthorize : AuthorizeAttribute
    {
        public PlayerTypeEnum Authorized { get; set; }

        public SociatisAuthorize(PlayerTypeEnum playerType)
        {
            Authorized = playerType;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (TransactionScope tsSuppressed = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var player = SessionHelper.LoggedCitizen;

                if (player == null)
                {
                    tsSuppressed.Complete();
                    return false;
                }

                var playerType = player.GetPlayerType();

                if (!isAuthorized(playerType))
                {
                    tsSuppressed.Complete();
                    return false;
                }

                tsSuppressed.Complete();
                return true;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "Account",
                        action = "Login"
                    })
                );
        }

        private bool isAuthorized(PlayerTypeEnum playerType)
        {
            if ((int)playerType >= (int)Authorized)
                return true;
            return false;
        }
    }
}