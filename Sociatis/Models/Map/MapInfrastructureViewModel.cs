using Entities;
using Sociatis.Models.Map.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class MapDevelopementViewModel : GeoMapViewModel<GeoDevelopementMapItem>
    {
        public MapDevelopementViewModel() { }
        public MapDevelopementViewModel(List<Region> regions)
        {
            Items = regions.Select(region => new GeoDevelopementMapItem(region)).ToList();
        }
    }
}