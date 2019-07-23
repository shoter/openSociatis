using Entities.enums;
using Entities.structs.jobOffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.JobOffers
{
    public class JobMarketViewModel
    {
        public int? CountryID { get; set; }
        public List<SelectListItem> Countries { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> WorkTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OfferTypes { get; set; } = new List<SelectListItem>();
        public JobMarketBestViewModel MinMax { get; set; }

        public JobMarketViewModel(int countryID, IEnumerable<Entities.Country> countries, CountryBestJobOffers bestJobs)
        {
            CountryID = countryID;

            foreach(var country in countries)
            {
                Countries.Add(new SelectListItem()
                {
                    Value = country.ID.ToString(),
                    Text = country.Entity.Name
                });

                if(country.ID == CountryID)
                {
                    MinMax = new JobMarketBestViewModel(country, bestJobs);
                }
            }
            foreach(WorkTypeEnum workType in Enum.GetValues(typeof(WorkTypeEnum)))
            {
                WorkTypes.Add(new SelectListItem()
                {
                    Value = ((int)workType).ToString(),
                    Text = workType.ToString(),
                    Selected = (workType == WorkTypeEnum.Any)
                });
            }

            foreach(JobOfferTypeEnum offerType in Enum.GetValues(typeof(JobOfferTypeEnum)))
            {
                OfferTypes.Add(new SelectListItem()
                {
                    Text = offerType.ToString(),
                    Value = ((int)offerType).ToString()
                });

            }


        }

    }
}