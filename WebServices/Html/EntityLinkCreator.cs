using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Weber.Helpers;

namespace Weber.Html
{
    public class EntityLinkCreator
    {
        public static MvcHtmlString Create(Entity entity, string @class = null)
        {
            return LinkCreator.Create(
                name: entity.Name,
                action: "View",
                controller: "Entity",
                routeValues: new { entityID = entity.EntityID });
            
        }

    }
}