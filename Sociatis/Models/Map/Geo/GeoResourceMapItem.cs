using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map.Geo
{
    public class GeoResourceMapItem : GeoMapItem
    {
        public string RegionName { get; set; }
        public int ResourceQuality { get; set; }

        public GeoResourceMapItem(Region region, int resourceQuality) : base(region.Polygon)
        {
            RegionName = region.Name;
            ResourceQuality = resourceQuality;
        }
    }
}