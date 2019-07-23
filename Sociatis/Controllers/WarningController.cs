using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Warnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebServices;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Controllers
{
    public class WarningController : ControllerBase
    {
        private readonly IWarningRepository warningRepository;

        public WarningController(IWarningRepository warningRepository, IPopupService popupService) : base(popupService)
        {
            this.warningRepository = warningRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public RedirectToRouteResult IndexPost(PagingParam pagingParam)
        {
            return RedirectToAction("Index", new RouteValueDictionary {  { "pagingParam.PageNumber", pagingParam.PageNumber } });
        }

        [Route("Warning/{pagingParam.PageNumber:int=1}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        public ViewResult Index(PagingParam pagingParam)
        {
            var entity = SessionHelper.CurrentEntity;
            var warnings = warningRepository.Where(warning => warning.EntityID == entity.EntityID)
                .OrderByDescending(warning => warning.ID)
                .Apply(pagingParam)
                .ToList();

            

            

            var vm = new IndexWarningViewModel(warnings, pagingParam);

            foreach (var warning in warnings)
                warning.Unread = false;

            warningRepository.SaveChanges();

            return View(vm);
        }

        [Route("Warning/Delete")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult DeleteWarnings(WarningDeleteViewModel[] warnings)
        {
            if (warnings != null)
            {
                foreach (var warning in warnings)
                {
                    warningRepository.Remove(warning.WarningID);
                }

                warningRepository.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}