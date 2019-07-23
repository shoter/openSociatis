using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.structs;
using Entities;
using Entities.Repository;

namespace WebServices.PathFinding
{
    public class TradeRegionSelector : IPathFindingRegionSelector
    {
        public int[] NoCrossCountries { get; protected set; }

        public TradeRegionSelector(Country buyerCountry, Country sellerCountry, IEmbargoRepository embargoRepository)
        {
            List<int> embargoedCountriesIDs = getEmbargoedCountriesIDs(buyerCountry, embargoRepository);
            embargoedCountriesIDs.AddRange(getEmbargoedCountriesIDs(sellerCountry, embargoRepository));

            NoCrossCountries = embargoedCountriesIDs
                .Distinct()
                .ToArray();
        }


        private List<int> getEmbargoedCountriesIDs(Country buyerCountry, IEmbargoRepository embargoRepository)
        {
            return embargoRepository.Where(e =>
            e.Active && e.EmbargoedCountryID == buyerCountry.ID)
            .Select(e => e.CreatorCountryID)
            .Distinct()
            .ToList();
        }

        public bool IsPassableRegion(Neighbour neighbour)
        {
            return NoCrossCountries.Contains(neighbour.Region.ID) == false;
        }
    }
}
