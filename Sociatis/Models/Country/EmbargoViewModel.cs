using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Country
{
    public class EmbargoViewModel
    {
        public int EmbargoID { get; set; }

        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public ImageViewModel CountryFlag { get; set; }
        public string StartDateTime { get; set; }
        public MoneyViewModel Upkeep { get; set; }

        public string CreatorCountryName { get; set; }
        public int CreatorID { get; set; }
        public ImageViewModel CreatorFlag { get; set; }

        public EmbargoViewModel(Embargo embargo, IEmbargoService embargoService)
        {
            EmbargoID = embargo.ID;

            CountryName = embargo.EmbargoedCountry.Entity.Name;
            CountryID = embargo.EmbargoedCountryID;
            CountryFlag = Images.GetCountryFlag(CountryID).VM;

            CreatorCountryName = embargo.CreatorCountry.Entity.Name;
            CreatorID = embargo.CreatorCountryID;
            CreatorFlag = Images.GetCountryFlag(CreatorID).VM;

            StartDateTime = string.Format("day {0} {1}", embargo.StartDay, embargo.StartTime.ToShortTimeString());

            Upkeep = new MoneyViewModel(CurrencyTypeEnum.Gold, (decimal)embargoService.GetEmbargoCost(embargo.CreatorCountry, embargo.EmbargoedCountry));
            
        }
    }
}