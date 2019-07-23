using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class RegionController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IHospitalService hospitalService;
        private readonly ICountryPresidentService countryPresidentService;
        private readonly IHospitalRepository hospitalRepository;

        public RegionController(IRegionRepository regionRepository, IPopupService popupService, IHospitalService hospitalService,
            ICountryPresidentService countryPresidentService, IHospitalRepository hospitalRepository) : base(popupService)
        {
            this.regionRepository = regionRepository;
            this.hospitalService = hospitalService;
            this.countryPresidentService = countryPresidentService;
            this.hospitalRepository = hospitalRepository;
        }

        [Route("Region/{regionID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult View(int regionID)
        {
            var region = regionRepository.GetById(regionID);

            if(region == null)
            {
                return RedirectToHomeWithError("Region does not exist!");
            }

            var vm = new RegionViewModel(region, hospitalService, hospitalRepository);

            return View(vm);
        }


        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult SetSpawnState(int regionID, bool enabled)
        {
            var region = regionRepository.GetById(regionID);

            var result = countryPresidentService.CanManageSpawn(region.Country, SessionHelper.CurrentEntity, region, enabled);
            if (result.IsError)
                return JsonError(result);

            countryPresidentService.ManageSpawn(region, enabled);

            return JsonSuccess("Spawn " + (enabled ? "enabled" : "disabled"));
        }
    }
}