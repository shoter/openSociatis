using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    /// <summary>
    /// Represents Polygon which is or can be saved in Db
    /// </summary>
    public class MapDatabasePolygonViewModel : MapPolygonViewModel
    {
        public int? PolygonID { get; set; }
        public MapDatabasePolygonViewModel() { }
        public MapDatabasePolygonViewModel(Polygon polygon)
        {
            PolygonID = polygon.ID;
            Color = polygon.Color;
            Opacity = (double)polygon.Opacity;
            FillColor = polygon.FillColor;
            FillOpacity = (double)polygon.FillOpacity;
            Weight = (double)polygon.Weight;

            LoadPoints(polygon);
        }

        public void LoadPoints(Polygon polygon)
        {
            foreach (var point in polygon.PolygonPoints)
                Points.Add(new MapPointViewModel(point));
        }
    }
}