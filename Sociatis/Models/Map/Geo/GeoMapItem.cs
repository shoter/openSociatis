using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map.Geo
{
    public class GeoMapItem
    {
        public List<MapPointViewModel> Points { get; set; } = new List<MapPointViewModel>();

        public GeoMapItem() { }

        public GeoMapItem(Polygon polygon)
        {
            Points = polygon.PolygonPoints.Select(point => new MapPointViewModel(point)).ToList();
        }
    }
}