using Entities.enums;
using Entities.Extensions;
using Entities.Models.Businesses;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Businesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository businessRepository;
        public BusinessController(IPopupService popupService, IBusinessRepository businessRepository) : base(popupService)
        {
            this.businessRepository = businessRepository;

        }

        [Route("Business/{businessID:int}-{businessType:int}/")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult View(int businessID, BusinessTypeEnum businessType)
        {
            switch (businessType)
            {
                case BusinessTypeEnum.Hotel:
                case BusinessTypeEnum.Company:
                    return RedirectToAction("View", "Entity", new { ID = businessID });
            }
            throw new NotImplementedException();
        }

        [Route("Business")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Index()
        {
            if (SessionHelper.CurrentEntity.IsNot(EntityTypeEnum.Citizen))
                return RedirectBackWithError("You can view your businesses only as citizen!");

            var businesses = businessRepository.GetBusinesses(SessionHelper.CurrentEntity.EntityID);

            var vm = new BusinessIndexViewModel(businesses);
            return View(vm);
        }
    }
}