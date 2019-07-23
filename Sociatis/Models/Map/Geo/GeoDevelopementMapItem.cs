using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map.Geo
{
    public class GeoDevelopementMapItem : GeoMapItem
    {
        public string RegionName { get; set; }
        public double DevelopementValue { get; set; }

        public GeoDevelopementMapItem(Region region) : base(region.Polygon)
        {
            RegionName = region.Name;
            DevelopementValue = (double)region.Development;
        }
    }
}