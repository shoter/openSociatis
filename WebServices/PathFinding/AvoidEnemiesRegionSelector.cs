using Entities;
using Entities.Repository;
using Entities.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.PathFinding
{
    public class AvoidEnemiesRegionSelector : IPathFindingRegionSelector
    {
        public int[] NoCrossCountries { get; protected set; }

        public int DestinationID { get; protected set; }

        public AvoidEnemiesRegionSelector(Country attackerCountry, IWarRepository warRepository, int destinationID)
        {
            List<int> countryInWar = getCountryInWar(attackerCountry, warRepository);
            DestinationID = destinationID;

            NoCrossCountries = countryInWar
                .Distinct()
                .ToArray();
        }

        private List<int> getCountryInWar(Country buyerCountry, IWarRepository warRepository)
        {
            List<int> countryIDs = warRepository.GetDirectWarsForCountry(buyerCountry.ID)
                .Select(w => w.AttackerCountryID == buyerCountry.ID ? w.DefenderCountryID : w.AttackerCountryID)
                .Distinct()
                .ToList();

            countryIDs.AddRange(warRepository.GetDirectWarsForCountry(buyerCountry.ID)
                  .SelectMany(w => w.CountryInWars.Where(c => c.IsAttacker == (buyerCountry.ID == w.AttackerCountryID)))
                  .Select(ciw => ciw.CountryID)
                  .Distinct()
                  .ToList());

            return countryIDs;
        }

        public bool IsPassableRegion(Neighbour neighbour)
        {
            if (neighbour.Region.ID == DestinationID)
                return true;

            return neighbour.Region.CountryID.HasValue == false || NoCrossCountries.Contains(neighbour.Region.CountryID.Value) == false;
        }
    }
}
