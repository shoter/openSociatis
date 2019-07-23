using Common;
using Common.Transactions;
using Entities.enums;
using Entities.Repository;
using MoreLinq;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs.Hospitals;
using WebUtils.Attributes;
using WebUtils.Mvc;

namespace Sociatis.Controllers
{
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalRepository hospitalRepository;
        private readonly IHospitalService hospitalService;
        private readonly ICompanyRepository companyRepository;
        private readonly ITransactionScopeProvider transactionScopeProvider;

        public HospitalController(IPopupService popupService, IHospitalRepository hospitalRepository, IHospitalService hospitalService,
            ITransactionScopeProvider transactionScopeProvider, ICompanyRepository companyRepository) : base(popupService)
        {
            this.hospitalRepository = hospitalRepository;
            this.hospitalService = hospitalService;
            this.transactionScopeProvider = transactionScopeProvider;
            this.companyRepository = companyRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        [HttpPost]
        public ActionResult Heal(int hospitalID)
        {
            var hospital = hospitalRepository.GetById(hospitalID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = hospitalService.CanHealCitizen(citizen, hospital);
            if (result.IsError)
            {
                return RedirectBackWithError(result);
            }

            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                hospitalService.HealCitizen(citizen, hospital);
                trs.Complete();

                AddSuccess("You have been healed!");
                return RedirectBack();
            }
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/Hospital/{hospitalID:int}/Manage")]
        public ActionResult Manage(int hospitalID)
        {
            var manageInfo = hospitalRepository.GetHospitalManageInfo(hospitalID);
            var hospital = hospitalRepository.Include(h => h.Company)
                .SingleOrDefault(h => h.CompanyID == hospitalID);
            var entity = SessionHelper.CurrentEntity;

            var result = hospitalService.CanManageHospital(entity, hospital);
            if (result.IsError)
                return RedirectBackWithError(result);

            var vm = new ManageHospitalViewModel(hospital, manageInfo);
            return View(vm);
        }



        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [AjaxOnly]
        public JsonResult UpdateHospital( UpdateHospitalViewModel vm,[ModelBinder(typeof(ONIArrayModelBinder))] UpdateHospitalNationalityPriceViewModel[] countryOptions)
        {
            if (vm.HealingFree == true || vm.HealingPrice <= 0)
                vm.HealingPrice = null;

            try
            {
                var hospital = hospitalRepository.GetById(vm.HospitalID);
                var entity = SessionHelper.CurrentEntity;

                var result = hospitalService.CanManageHospital(entity, hospital);
                if (result.IsError)
                    return JsonError(result);

                hospitalService.UpdateHospital(hospital, vm.HealingPrice, vm.HealingEnabled == true);

                hospitalService.UpdateHospitalNationalityOptions(hospital, countryOptions
                    .Select(x => new UpdateHospitalNationalityOption()
                    {
                        CountryID = x.CountryID,
                        Price = x.HealingFree ? null :  x.Price
                    })
                    .DistinctBy(x => x.CountryID));
                return JsonSuccess("Hospital updated!");
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }
        }
    }
}