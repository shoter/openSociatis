using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebServices.PathFinding
{
    public class BattlePathFindingCostCalculator : DefaultPassageCostCalculator
    {
        public int[] EnemyCountriesIDs { get; protected set; }
        public BattlePathFindingCostCalculator(War war, bool isAttacker, IRegionService regionService) : base(regionService)
        {
            EnemyCountriesIDs = new int[1 + war.CountryInWars.Where(ciw => ciw.IsAttacker == isAttacker).Count()];
            EnemyCountriesIDs[0] = isAttacker ? war.AttackerCountryID : war.DefenderCountryID;
            int i = 1;

            foreach(var country in war.CountryInWars.Where(ciw => ciw.IsAttacker == isAttacker).ToList())
            {
                EnemyCountriesIDs[i++] = country.CountryID;
            }
        }

        public override double CalculatePassageCost(Passage passage)
        {
            var developementRating = (double)passage.FirstRegion.Development + (double)passage.SecondRegion.Development ;

            if (isEnemyPassage(passage))
                developementRating /= 2;

            var cost = regionService.CalculateDistanceWithDevelopement(passage.Distance, developementRating);

            if (isEnemyPassage(passage))
                cost *= 5;

            return cost;
        }

        private bool isEnemyPassage(Passage passage)
        {
            return isEnemyRegion(passage.FirstRegion) || isEnemyRegion(passage.SecondRegion);
        }

        private bool isEnemyRegion(Region region)
        {
            return region.CountryID.HasValue && EnemyCountriesIDs.Contains(region.CountryID.Value);
        }
    }
}
