using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class MapPolygonViewModel
    {
        /// <summary>
        /// Defines if is polygon editable. (default: false)
        /// </summary>
        public bool Editable { get; set; } = false;
        /// <summary>
        /// Sets color of polygon's boundary. (default: #03f)
        /// </summary>
        public string Color { get; set; } = "#03f";
        /// <summary>
        /// Sets opacity of polygon. (default: 0.5)
        /// </summary>
        public double Opacity { get; set; } = 0.5;
        /// <summary>
        /// Sets color of polygon's fill. (default: #03f)
        /// </summary>
        public string FillColor { get; set; } = "#03f";
        /// <summary>
        ///  Sets opacity of polygon's fill. (1 - Fully opaque, 0 - Fully ransparent) (default: 0.2)
        /// </summary>
        public double FillOpacity { get; set; } = 0.2;
        /// <summary>
        /// Weight of boundary. (default: 5)
        /// </summary>
        public double Weight { get; set; } = 5;

        public List<MapPointViewModel> Points { get; set; } = new List<MapPointViewModel>();
    }
}