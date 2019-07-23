using Entities.enums;
using Sociatis.Helpers;
using Sociatis.Models.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class CountryRegionViewModel
    {
        public int RegionID { get; set; }
        public string Name { get; set; }
        public List<CountryRegionPassageViewModel> Passages { get; set; } = new List<CountryRegionPassageViewModel>();
        public double Developement { get; set; }
        public CountryRegionHospitalInformation Hospital { get; set; }

        public int DefenseSystemQuality { get; set; }
        public double Infrastructure { get; set; }
        public bool SpawnEnabled { get; set; }

        public List<RegionResourceViewModel> Resources = new List<RegionResourceViewModel>();

        public CountryRegionViewModel(Entities.Region region)
        {
            Name = region.Name;
            RegionID = region.ID;
            SpawnEnabled = region.CanSpawn;

            var passages = region.Passages.ToList();
            passages = passages.Concat(region.Passages1).ToList();

            foreach (var passage in passages)
                Passages.Add(new CountryRegionPassageViewModel(passage, region.ID));

            Developement = (double)region.Development;
            DefenseSystemQuality = region.DefenseSystemQuality;
            if (region.Hospital != null)
                Hospital = new CountryRegionHospitalInformation(region.Hospital);

            foreach(var resource in region.Resources.ToList())
            {
                Resources.Add(new RegionResourceViewModel(resource));
            }
        }

    }
}