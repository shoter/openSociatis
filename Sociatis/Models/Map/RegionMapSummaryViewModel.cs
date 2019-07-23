using Entities;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Map
{
    public class RegionMapSummaryViewModel
    {
        public string CountryName { get; set; }
        public int Population { get; set; }
        public List<RegionResourceMapSummaryViewModel> Resources { get; set; }
        public double Developement { get; set; }
        public int HospitalQuality { get; set; }
        public int DefenseSystemQuality { get; set; }
        public List<RegionNeigbourMapSummaryViewModel> Neighbours { get; set; }

        public RegionMapSummaryViewModel(Region region)
        {
            CountryName = region.Country.Entity.Name;
            Population = region.Citizens.Count;
            Resources = region.Resources.ToList().Select(r => new RegionResourceMapSummaryViewModel(r)).ToList();
            Developement = (double)region.Development * 20.0; //to %
            HospitalQuality = region?.Hospital?.Company?.Quality ?? 0;
            DefenseSystemQuality = region.DefenseSystemQuality;

            var persistentRegion = Persistent.Regions.GetById(region.ID);
            Neighbours = persistentRegion.GetNeighbours().Select(n => new RegionNeigbourMapSummaryViewModel(n)).ToList();
        }

    }
}