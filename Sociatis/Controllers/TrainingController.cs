using Entities.enums;
using Entities.Extensions;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class TrainingController : ControllerBase
    {
        private readonly ICitizenService citizenService;

        public TrainingController(ICitizenService citizenService, IPopupService popupService) : base(popupService)
        {
            this.citizenService = citizenService;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Training/")]
        public ActionResult Index()
        {
            if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen) == false)
                return RedirectToHomeWithError("You are not a citizen!");

            var vm = new TrainingIndexViewModel(SessionHelper.LoggedCitizen);
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Training/Train")]
        [HttpPost]
        public ActionResult Train()
        {
            if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen) == false)
                return RedirectToHomeWithError("You are not a citizen!");

            var citizen = SessionHelper.LoggedCitizen;

            if (citizen.HitPoints <= 0)
                return RedirectBackWithError("You do not have enough hitpoints!");

            if (citizen.Trained)
                return RedirectBackWithError("You had trained today!");
            double strengthGain = citizenService.Train(citizen);

            return View(new TrainingDoneViewModel(SessionHelper.LoggedCitizen, strengthGain));
        }
    }
}