using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Sociatis.Code.Constraints
{
    public class ResourceTypeEnumConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // You can also try Enum.IsDefined, but docs say nothing as to
            // is it case sensitive or not.
            var response = Enum.GetNames(typeof(ResourceTypeEnum)).Any(s => s.ToLowerInvariant() == values[parameterName].ToString().ToLowerInvariant());
            return response;
        }
    }
}