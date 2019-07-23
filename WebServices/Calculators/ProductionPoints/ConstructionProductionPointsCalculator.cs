using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Calculators.ProductionPoints
{
    public class ConstructionProductionPointsCalculator : DefaultProductionPointsCalculator
    {
        public ConstructionProductionPointsCalculator()
        {
        }

        public override double CalculateDistanceModifier(ProductionPointsCalculateArgs args)
        {
            if (args.Distance == 0)
                return 1f;

            return .4f + .6f / Math.Sqrt(args.Distance / 500f + 1f);
        }
    }
}
