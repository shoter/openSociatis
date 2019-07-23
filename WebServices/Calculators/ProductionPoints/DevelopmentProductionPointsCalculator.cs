using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Calculators.ProductionPoints
{
    public class DevelopmentProductionPointsCalculator : DefaultProductionPointsCalculator
    {
        public override double Calculate(ProductionPointsCalculateArgs args)
        {
            WorkTypeEnum workType = args.ProducedProduct.GetWorkType();
            double skill = args.Skill;
            double distanceModifier = CalculateDistanceModifier(args);
            double developmentModifier = CalculateDevelopmentModifier(args);
            double peopleModifier = CalculatePeopleModifier(args);
            double hpModifier = CalculateHealthModifier(args);

            return Math.Pow(Math.Pow((skill + 1) * distanceModifier, 0.89) * developmentModifier * peopleModifier, 0.125) * hpModifier / 50.0;
        }

        public override double CalculateDevelopmentModifier(ProductionPointsCalculateArgs args)
        {
            return Math.Pow(6 - args.Development, 3);
        }

        public override double CalculatePeopleModifier(ProductionPointsCalculateArgs args)
        {

            if (args.PeopleCount <= 10)
                return 0.75 + args.PeopleCount * 0.25 / 10.0;
            
            return Math.Max(1.0 - (args.PeopleCount - 10) * 0.1, 0.1);
        }

    }
}
