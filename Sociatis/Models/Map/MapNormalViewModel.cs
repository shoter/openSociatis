using Entities;
using Sociatis.Models.Map.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Map
{
    public class MapNormalViewModel : GeoMapViewModel<GeoPoliticalMapItem>
    {
        public MapNormalViewModel() { }
        public MapNormalViewModel(List<Region> regions)
        {
            Items = regions.Where(r => r.Polygon != null).Select(region => new GeoPoliticalMapItem(region)).ToList();
        }
    }
}