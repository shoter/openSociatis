using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class CountryRegionPassageViewModel
    {
        public string ConnectedToName { get; set; }
        public int Distance { get; set; }
        public double Developement { get; set; }

        public CountryRegionPassageViewModel(Entities.Passage passage, int originRegionID)
        {
            if (passage.SecondRegionID == originRegionID)
            {
                ConnectedToName = passage.FirstRegion.Name;
                Developement = (double)passage.FirstRegion.Development;
            }
            else
            {
                ConnectedToName = passage.SecondRegion.Name;
                Developement = (double)passage.SecondRegion.Development;
            }

            Distance = passage.Distance;
            
        }
    }
}