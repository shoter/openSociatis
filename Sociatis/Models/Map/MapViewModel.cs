using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class MapViewModel
    {
        public List<MapRegionViewModel> Regions { get; set; }

        public MapViewModel() { }
        public MapViewModel(List<Region> regions)
        {
            Regions = regions
                .Where(r => r.PolygonID.HasValue)
                .Select(r => new MapRegionViewModel(r)).ToList();
        }
    }
}