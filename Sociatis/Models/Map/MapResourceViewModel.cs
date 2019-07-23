using Entities;
using Entities.enums;
using Sociatis.Code;
using Sociatis.Models.Map.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Map
{
    public class MapResourceViewModel : GeoMapViewModel<GeoResourceMapItem>
    {
        public List<string> ColorScale { get; set; } = new List<string>() { "#FFFFFF" };
        public string ResourceName { get; set; }
        public MapResourceViewModel() { }
        public MapResourceViewModel(List<Region> regions, Dictionary<int, int> regionToResource, ResourceTypeEnum resourceType)
        {
            ColorScale.AddRange(Colors.ResourceColors[resourceType]);
            ResourceName = resourceType.ToHumanReadable();
            Items = regions
                .Where(r => r.Polygon != null)
                .Select(region => new GeoResourceMapItem(region, regionToResource[region.ID])).ToList();
        }
    }
}