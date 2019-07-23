using Entities;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebServices;

namespace Sociatis.Models.Map.Geo
{
    public class GeoPoliticalMapItem : GeoMapItem
    {
        public string RegionName { get; set; }
        public int RegionID { get; set; }
        public string RegionColor { get; set; }
        public List<int> Neighbours { get; set; }

        public GeoPoliticalMapItem(Region region) : base(region.Polygon)
        {
            RegionName = region.Name;
            RegionName = Regex.Replace(RegionName, ".{10}", "$0<br/>");
            RegionID = region.ID;

            RegionColor = Persistent.Countries.First(c => c.ID == region.CountryID).Color;

            Neighbours = Persistent.Regions.GetById(region.ID).GetNeighbours()
                .Select(n => n.Region.ID)
                .Distinct()
                .ToList();
        }
    }
}