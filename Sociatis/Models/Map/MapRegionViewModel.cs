using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class MapRegionViewModel : MapDatabasePolygonViewModel
    {
        public string RegionName { get; set; }
        public int? RegionID { get; set; }

        public List<int> Fertility { get; set; } = new List<int>();

        public MapRegionViewModel(Region region) : base(region.Polygon)
        {
            RegionID = region.ID;
            RegionName = region.Name;

            var resources = region.Resources.ToDictionary(r => r.ResourceTypeID, r => r.ResourceQuality);

            foreach (var res in Enum.GetValues(typeof(ResourceTypeEnum)).Cast<int>())
            {
                if (resources.ContainsKey(res))
                    Fertility.Add(resources[res]);
                else
                    Fertility.Add(0);
            }
        }

        public MapRegionViewModel() { }
    }
}