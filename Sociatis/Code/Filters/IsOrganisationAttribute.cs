using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Code.Filters
{
    public class IsOrganisationAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity == null)
                return false;

            return entity.EntityTypeID == (int)EntityTypeEnum.Organisation;
        }
    }
}