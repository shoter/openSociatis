using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebServices.PathFinding
{
    public class DefaultPassageCostCalculator : IPathFindingCostCalculator
    {
        protected readonly IRegionService regionService;
        public DefaultPassageCostCalculator(IRegionService regionService)
        {
            this.regionService = regionService;
        }
        public virtual double CalculatePassageCost(Passage passage)
        {
            return regionService.CalculateDistanceWithDevelopement(
                distance: passage.Distance,
                developement: (double)passage.FirstRegion.Development + (double)passage.SecondRegion.Development);

        }
    }
}
