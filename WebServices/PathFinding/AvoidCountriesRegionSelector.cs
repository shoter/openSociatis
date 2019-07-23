using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.structs;

namespace WebServices.PathFinding
{
    public class AvoidCountriesRegionSelector : IPathFindingRegionSelector
    {
        public int[] BlockedCountriesIDs { get; private set; }

        public AvoidCountriesRegionSelector(params int[] blockedCountriesIDs)
        {
            this.BlockedCountriesIDs = blockedCountriesIDs;
        }


        public bool IsPassableRegion(Neighbour neighbour)
        {
            return neighbour.Region.CountryID.HasValue == false ||
                BlockedCountriesIDs.Contains(neighbour.Region.CountryID.Value);
        }
    }
}
