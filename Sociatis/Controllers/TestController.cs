using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class TestController : ControllerBase
    {
        public TestController(IPopupService popupService) : base(popupService) { }
        [SociatisAuthorize(PlayerTypeEnum.Moderator)]
        public ViewResult ListMyJob()
        {
            var me = SessionHelper.LoggedCitizen;

            return View(me);
        }

        [SociatisAuthorize(PlayerTypeEnum.Moderator)]
        public ActionResult TestMarket()
        {
            return View();
        }


        [SociatisAuthorize(PlayerTypeEnum.Moderator)]
        public ViewResult Popup()
        {
            AddInfo("Info");
            AddError("Error");
            AddSuccess("Sukces");
            AddWarning("waring");
            return View();
        }
    }
}