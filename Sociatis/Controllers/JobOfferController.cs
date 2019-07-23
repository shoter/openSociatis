using Common.Exceptions;
using Common.Extensions;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Entities.Selector;
using Entities.structs.jobOffers;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.JobOffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils;
using WebUtils.Attributes;
using WebUtils.Extensions;

namespace Sociatis.Controllers
{
    public class JobOfferController : ControllerBase
    {
        private readonly IJobOfferRepository jobOfferRepository;
        private readonly ICountryRepository countryRepository;

        public JobOfferController(IJobOfferRepository jobOfferRepository, ICountryRepository countryRepository, 
            IPopupService popupService) : base(popupService)
        {
            this.jobOfferRepository = jobOfferRepository;
            this.countryRepository = countryRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult JobMarket()
        {
            var entity = SessionHelper.CurrentEntity;
            var countries = countryRepository.GetAll();
            var entityCountryID = entity.GetCurrentCountry().ID;

            var bestOffers = jobOfferRepository.GetBestAndWorstJobOffers(entityCountryID, WorkTypeEnum.Any);
            var vm = new JobMarketViewModel(entityCountryID, countries, bestOffers);
            return View(vm);
        }

        [AjaxOnly]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult JobMarketMinMax(int countryID, WorkTypeEnum workType)
        {
            try
            {
                var country = countryRepository.GetById(countryID);
                if (country == null)
                    throw new UserReadableException("Country does not exist!");

                var bestOffers = jobOfferRepository.GetBestAndWorstJobOffers(country.ID, workType);

                var vm = new JobMarketBestViewModel(country, bestOffers);
                return JsonData(vm);


            } catch(Exception e)
            { 
                return JsonDebugOnlyError(e);
            }
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public PartialViewResult Offers(int countryID, double minSkill, double maxSkill, double minSalary, double maxSalary, bool? sameRegion, JobOfferTypeEnum offerType, WorkTypeEnum workTypeID, PagingParam pagingParam)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var offers = jobOfferRepository.Where(o => o.CountryID == countryID);

            if (offerType != JobOfferTypeEnum.Both)
                offers = offers.Where(o => o.TypeID == (int)offerType);
            if (workTypeID != WorkTypeEnum.Any)
                offers = offers.Where(o => o.Company.WorkTypeID == (int)workTypeID);
            //if (minSkill.HasValue)
                offers = offers.Where(o => o.MinSkill >= (decimal)minSkill);
           // if (maxSkill.HasValue)
                offers = offers.Where(o => o.MinSkill <= (decimal)maxSkill);
            //if (minSalary.HasValue)
                offers = offers.Where(o => o.NormalJobOffer.Salary >= (decimal)minSalary || o.ContractJobOffer.MinSalary >= (decimal)minSalary);
            //if(maxSalary.HasValue)
                offers = offers.Where(o => o.NormalJobOffer.Salary <= (decimal)maxSalary || o.ContractJobOffer.MinSalary <= (decimal)maxSalary);

            if(sameRegion == true)
            {
                int? regionID = SessionHelper.CurrentEntity.GetCurrentRegion()?.ID;
                offers = offers.Where(o => o.Company.RegionID == regionID);
            }


            JobOffersDOMSelector selector = new JobOffersDOMSelector();

            IList<JobOfferDOM> processed = offers
                .Apply(selector)
                .OrderByDescending(o => o.NormalSalary)
                .Apply(pagingParam)
                .ToList();

            var vm = new JobMarketOfferListViewModel(processed, countryID, (int)workTypeID, (int)offerType, minSalary, maxSalary, minSkill, maxSkill,
                includeRegionInformation: sameRegion != true, pagingParam: pagingParam);

            return PartialView(vm);
        }
    }
}