using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sociatis.ActionFilters
{
    public class EntityAuthorizeAttribute : ActionFilterAttribute
    {
        public EntityTypeEnum[] AllowedEntityTypes { get; set; }

        public EntityAuthorizeAttribute(params EntityTypeEnum[] allowedEntities)
        {
            AllowedEntityTypes = allowedEntities;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity == null)
                HandleUnauthorizedRequest(filterContext);

            var entityType = entity.GetEntityType();

            if (!isAuthorized(entityType))
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        protected void HandleUnauthorizedRequest(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "Home",
                        action = "Index"
                    })
                );
        }

        private bool isAuthorized(EntityTypeEnum entityType)
        {
            return AllowedEntityTypes.Contains(entityType);
        }
    }
}