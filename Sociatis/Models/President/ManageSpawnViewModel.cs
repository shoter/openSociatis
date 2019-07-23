using Entities.Models.Regions;
using Sociatis.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.President
{
    public class ManageSpawnViewModel
    {
        public CountryInfoViewModel Info { get; set; }


        public ManageSpawnViewModel(Entities.Country country, IList<RegionSpawnInformation> spawnInformations)
        {
            Info = new CountryInfoViewModel(country);
        }
    }
}