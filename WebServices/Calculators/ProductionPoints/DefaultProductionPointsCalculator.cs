using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Extensions;
using System.Threading.Tasks;
using Entities.Repository;

namespace WebServices.Calculators.ProductionPoints
{
    public class DefaultProductionPointsCalculator : IProductionPointsCalculator
    {

        public DefaultProductionPointsCalculator()
        {

        }

        public virtual  double Calculate(ProductionPointsCalculateArgs args)
        {
            WorkTypeEnum workType = args.ProducedProduct.GetWorkType();
            double skill = args.Skill;
            var hpModifier = CalculateHealthModifier(args);
            double distanceModifier = CalculateDistanceModifier(args);
            double qualityModifier = CalculateQualityModifier(args);
            double productModifier = CalculateProductModifier(args);
            double developmentModifier = CalculateDevelopmentModifier(args);
            double peopleModifier = CalculatePeopleModifier(args);
            double rawMultiplier = 1.0;
            if (args.ProducedProduct.IsRaw())
                rawMultiplier = CalculateRawMultiplier(args);


            return (Math.Pow((skill * productModifier + 1) * qualityModifier * distanceModifier, 0.9 - (double)(args.Quality / 100.0)) 
                * rawMultiplier * developmentModifier) * peopleModifier * hpModifier * 1.5;
        }

        public virtual double CalculateDistanceModifier(ProductionPointsCalculateArgs args)
        {
            if (args.Distance == 0)
                return 1f;

            return .6f + .4f / Math.Sqrt(args.Distance / 1000f + 1f);
        }

        public virtual double CalculatePeopleModifier(ProductionPointsCalculateArgs args)
        {
            int firstStage = 4;
            int secondStage = 20;
            double inc2Modifier = 0.005;
            double dec = 0.05;
            double max = 0.5;

            switch (args.CompanyType)
            {
                case CompanyTypeEnum.Producer:
                    secondStage = 40;
                    break;
                case CompanyTypeEnum.Shop:
                    secondStage = 50;
                    inc2Modifier = 0.025;
                    break;
                case CompanyTypeEnum.Construction:
                    inc2Modifier = 0.015;
                    dec = 0.01;
                    break;
            }

            if (args.PeopleCount <= firstStage)
                return 0.75 + 0.25 / firstStage * args.PeopleCount;
            else if (args.PeopleCount > firstStage && args.PeopleCount <= secondStage)
                return 1 + inc2Modifier * (args.PeopleCount - firstStage);
            else
                return Math.Max(1 + inc2Modifier * (secondStage - firstStage) - dec * (args.PeopleCount - secondStage), max);
        }

        public virtual double CalculateProductModifier(ProductionPointsCalculateArgs args)
        {
            double productModifier = 1.0;

            switch (args.ProducedProduct)
            {
                case ProductTypeEnum.Paper:
                    productModifier = 3.5;
                    break;
                case ProductTypeEnum.SellingPower:
                    productModifier = 7; break;
                case ProductTypeEnum.Fuel:
                    productModifier = 1.5;
                    break;
            }

            return productModifier;
        }

        public virtual double CalculateQualityModifier(ProductionPointsCalculateArgs args)
        {
            double qualityDivider = 1.0;

            switch (args.Quality)
            {
                case 2:
                    qualityDivider = 1.5;
                    break;
                case 3:
                    qualityDivider = 2.25;
                    break;
                case 4:
                    qualityDivider = 3.375;
                    break;
                case 5:
                    qualityDivider = 5.0;
                    break;
            }

            return 1.0 / qualityDivider;
        }

        public virtual double CalculateRawMultiplier(ProductionPointsCalculateArgs args)
        {
            double rawMultiplier = 1.0;

            if (args.WorkType == WorkTypeEnum.Raw)
            {
                ResourceTypeEnum resourceType = args.ResourceType.Value;

                rawMultiplier = GetRawMultiplierForResourceQuality(args.ResourceQuality);
            }

            return rawMultiplier;
        }

        public static double GetRawMultiplierForResourceQuality(int? resourceQuality)
        {
            if (resourceQuality == null)
                return 0.1;
            else switch (resourceQuality.Value)
                {
                    case 1:
                        return 1.0;
                    case 2:
                        return 1.3;
                    case 3:
                        return 1.8;
                    case 4:
                        return 2.5;
                }

            throw new Exception("It should not happen");
        }

        public virtual double CalculateDevelopmentModifier(ProductionPointsCalculateArgs args)
        {
            return Math.Log(Math.Pow(args.Development * Math.Sin((args.Development + 1.6) / 5 * Math.PI / 2), 2) + 1, 10) + 0.8;
        }

        public double CalculateHealthModifier(ProductionPointsCalculateArgs args)
        {
            double x = args.HitPoints;
            x = (x - 50.0) / 20.0;
            double exp = Math.Exp(x);
            return Math.Min(1.0,
                exp / (exp + 1) + 0.08);
        }
    }
}
