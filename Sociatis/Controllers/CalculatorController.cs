using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Models.Calculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.PathFinding;

namespace Sociatis.Controllers
{
    public class CalculatorController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IRegionService regionService;
        private readonly IRegionRepository regionRepository;
        private readonly ICompanyService companyService;
        public CalculatorController(IPopupService popupService, IProductService productService, IRegionService regionService, IRegionRepository regionRepository,
            ICompanyService companyService) : base(popupService)
        {
            this.productService = productService;
            this.regionService = regionService;
            this.regionRepository = regionRepository;
            this.companyService = companyService;
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CalculateFuelCost()
        {
            var vm = new CalculateFuelCostViewModel();
            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CalculateFuelCost(CalculateFuelCostViewModel vm)
        {

            if (ModelState.IsValid)
            {
                if (vm.UseRegions)
                {
                    if (vm.StartRegionID.HasValue == false || vm.EndRegionID.HasValue == false)
                    {
                        ModelState.AddModelError("", "Regions cannot be nothing if you are using them!");
                        return View(vm);
                    }
                    var startRegion = regionRepository.GetById(vm.StartRegionID.Value);
                    var endRegion = regionRepository.GetById(vm.EndRegionID.Value);
                    vm.Distance = regionService.GetPathBetweenRegions(startRegion, endRegion, new DefaultRegionSelector(), new DefaultPassageCostCalculator(regionService)).Distance;
                }

                vm.Result = (int)Math.Ceiling(productService.GetFuelCostForTranposrt(vm.Distance.Value, (ProductTypeEnum)vm.ProductID, vm.Quality, vm.Amount));
            }
            return View(vm);
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CalculateProductionPoints()
        {
            CalculateProductionPointsViewModel vm = new CalculateProductionPointsViewModel();
            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CalculateProductionPoints(CalculateProductionPointsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Result = companyService.GetProductionPoints((ProductTypeEnum)vm.ProducedProductID, vm.HitPoints, vm.Skill, vm.Distance, vm.Quality, vm.ResourceQuality ?? 0, vm.Development, vm.PeopleCount);
            }
            return View(vm);
        }


        public JsonResult CanUseQuality(int productTypeID)
        {
            var productType = (ProductTypeEnum)productTypeID;
            var canUseQuality = productService.CanUseQuality(productType);

            return JsonData(canUseQuality);
        }

        public JsonResult CanUseFertility(int productTypeID)
        {
            var productType = (ProductTypeEnum)productTypeID;
            var canUseFertility = productService.CanUseFertility(productType);

            return JsonData(canUseFertility);
        }
    }
}