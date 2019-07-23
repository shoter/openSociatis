using Entities.structs.jobOffers;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebUtils;

namespace Sociatis.Models.JobOffers
{
    public class JobMarketOfferListViewModel
    {
        public List<JobMarketOfferViewModel> Offers { get; set; } = new List<JobMarketOfferViewModel>();

        public PagingParam PParam { get; set; }
        public int CountryID { get; set; }
        public int WorkTypeID { get; set; }
        public int OfferType { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public double MinSkill { get; set; }
        public double MaxSkill { get; set; }
        public bool IncludeRegionInformation { get; set; }
        public bool IsCitizen { get; set; }

        public JobMarketOfferListViewModel(IList<JobOfferDOM> offers, int countryID, int workTypeID, int offerType, double minSkill, double maxSkill, double minSalary, double maxSalary, bool includeRegionInformation, PagingParam pagingParam)
        {
            PParam = pagingParam;
            CountryID = countryID;
            WorkTypeID = workTypeID;
            OfferType = offerType;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            MinSkill = minSkill;
            MaxSkill = maxSkill;
            var currency = Persistent.Countries.GetCountryCurrency(countryID);
 
            foreach(var offer in offers)
            {
                Offers.Add(new JobMarketOfferViewModel(offer, currency));
            }

            IncludeRegionInformation = includeRegionInformation;
            IsCitizen = (SessionHelper.CurrentEntity.EntityID == SessionHelper.LoggedCitizen.ID);
        }
    }
}