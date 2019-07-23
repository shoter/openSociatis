using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class CountryRegionsListViewModel
    {
        public List<CountryRegionViewModel> Regions { get; set; } = new List<CountryRegionViewModel>();
        public CountryInfoViewModel Info { get; set; }

        public CountryRegionsListViewModel(Entities.Country country)
        {
            foreach (var region in country.Regions
                .OrderBy(r => r.Name)
                .ToList())
                Regions.Add(new CountryRegionViewModel(region));

            Info = new CountryInfoViewModel(country);
        }
    }
}