using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.PathFinding
{
    public class WarsPenaltyCostCalculator : DefaultPassageCostCalculator
    {
        public int[] EnemyCountriesIDs { get; protected set; }
        public int[] IndirectEnemyCountriesIDs { get; protected set; }
        public WarsPenaltyCostCalculator(Citizen citizen, IWarRepository warRepository, IRegionService regionService) : base(regionService)
        {
            EnemyCountriesIDs = warRepository.GetDirectWarsForCountry(citizen.CitizenshipID)
                .Select(w => w.AttackerCountryID == citizen.CitizenshipID ? w.DefenderCountryID : w.AttackerCountryID)
                .ToArray();

            var indirectEnemiesIDs = warRepository.GetIndirectWarsForCountry(citizen.CitizenshipID)
                .SelectMany(w => w.CountryInWars)
                .Where(ciw => ciw.CountryID != citizen.CitizenshipID)
                .Select(ciw => ciw.CountryID)
                .ToList();

            indirectEnemiesIDs.AddRange(warRepository.GetDirectWarsForCountry(citizen.CitizenshipID)
                .SelectMany(w => w.CountryInWars)
                .Select(ciw => ciw.CountryID)
                .ToList());

            IndirectEnemyCountriesIDs = indirectEnemiesIDs.ToArray();
        }

        public override double CalculatePassageCost(Passage passage)
        {
            var developementRating = (double)passage.FirstRegion.Development + (double)passage.SecondRegion.Development;

            if (isEnemyPassage(passage, EnemyCountriesIDs))
                developementRating /= 5;
            else if (isEnemyPassage(passage, IndirectEnemyCountriesIDs))
                developementRating /= 2;

            var cost = regionService.CalculateDistanceWithDevelopement(passage.Distance, developementRating);


            if (isEnemyPassage(passage, EnemyCountriesIDs))
                cost *= 3;
            else if (isEnemyPassage(passage, IndirectEnemyCountriesIDs))
                cost *= 2;

            return cost;
        }

        private bool isEnemyPassage(Passage passage, int[] enemyArrayIDs)
        {
            return isEnemyRegion(passage.FirstRegion, enemyArrayIDs) || isEnemyRegion(passage.SecondRegion, enemyArrayIDs);
        }

        private bool isEnemyRegion(Region region, int[] enemyArrayIDs)
        {
            return region.CountryID.HasValue && enemyArrayIDs.Contains(region.CountryID.Value);
        }
    }
}
