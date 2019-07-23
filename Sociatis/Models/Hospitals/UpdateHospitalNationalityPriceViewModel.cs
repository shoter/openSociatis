using Entities.Models.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUtils.Mvc;

namespace Sociatis.Models.Hospitals
{
    public class UpdateHospitalNationalityPriceViewModel
    {
        public int CountryID { get; set; }
        public decimal? Price { get; set; }
        public bool HealingFree { get; set; }
        

        public UpdateHospitalNationalityPriceViewModel() { }
        public UpdateHospitalNationalityPriceViewModel(HospitalManageNationalityHealingOption option)
        {
            CountryID = option.CountryID;
            Price = option.HealingPrice;
            HealingFree = option.HealingPrice == null;
        }
    }
}