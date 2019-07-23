using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class MapPointViewModel
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public MapPointViewModel(PolygonPoint polygonPoint)
        {
            Latitude = polygonPoint.Latitude;
            Longitude = polygonPoint.Longitude;
        }

        public MapPointViewModel() { }
    }
}