using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map.Geo
{
    public class GeoMapViewModel<TGeoItem>
        where TGeoItem: GeoMapItem
    {
        public List<TGeoItem> Items { get; set; }
    }
}