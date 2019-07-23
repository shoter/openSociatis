using Entities.enums;
using Sociatis.Code.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace Sociatis
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            addResolvers(routes);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        private static void addResolvers(RouteCollection routes)
        {
            var resourceTypeEnumResolver = new DefaultInlineConstraintResolver();
            resourceTypeEnumResolver.ConstraintMap.Add(nameof(ResourceTypeEnum), typeof(ResourceTypeEnumConstraint));
            routes.MapMvcAttributeRoutes(resourceTypeEnumResolver);
        }
    }
}
