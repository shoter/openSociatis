using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Admin
{
    public class CountryColorViewModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryColor { get; set; }

        public CountryColorViewModel() { }
        public CountryColorViewModel(Entities.Country country)
        {
            CountryID = country.ID;
            CountryName = country.Entity.Name;
            CountryColor = country.Color;
        }
    }
}