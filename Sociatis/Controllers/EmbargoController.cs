using Entities.enums;
using Sociatis.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Controllers
{
    public class EmbargoController
    {
        [SociatisAuthorize(PlayerTypeEnum.Player)]
      //  [Route(")]
        public ActionResult Index(int countryID)
        {
            throw new NotImplementedException();
        }

    }
}