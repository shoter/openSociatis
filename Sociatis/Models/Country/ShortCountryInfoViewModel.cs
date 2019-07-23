using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class ShortCountryInfoViewModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public ImageViewModel CountryFlag { get; set; }

        public ShortCountryInfoViewModel(Entities.Country country)
        {
            CountryID = country.ID;
            CountryName = country.Entity.Name;
            CountryFlag = Images.GetCountryFlag(country).VM;
        }
    }
}