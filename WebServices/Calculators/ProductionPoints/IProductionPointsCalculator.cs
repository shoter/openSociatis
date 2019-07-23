using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Calculators.ProductionPoints
{
    public interface IProductionPointsCalculator
    {

        double Calculate(ProductionPointsCalculateArgs args);
        double CalculateHealthModifier(ProductionPointsCalculateArgs args);
        double CalculateDistanceModifier(ProductionPointsCalculateArgs args);
        double CalculatePeopleModifier(ProductionPointsCalculateArgs args);
        double CalculateProductModifier(ProductionPointsCalculateArgs args);
        double CalculateQualityModifier(ProductionPointsCalculateArgs args);
        double CalculateRawMultiplier(ProductionPointsCalculateArgs args);
        double CalculateDevelopmentModifier(ProductionPointsCalculateArgs args);
    }
}
