using Entities.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class RegionNeigbourMapSummaryViewModel
    {
        public string RegionName { get; set; }
        /// <summary>
        /// In kilometers
        /// </summary>
        public int Distance { get; set; }

        public RegionNeigbourMapSummaryViewModel(Neighbour neighbour)
        {
            Distance = neighbour.Passage.Distance;
            RegionName = neighbour.Region.Name;

            if (RegionName.Length > 13)
                RegionName = RegionName.Substring(0, 10) + "...";
        }
    }
}