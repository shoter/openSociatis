using Common.Maths;
using Entities;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.Calculators.ProductionPoints;

namespace Sociatis.Models.Companies
{
    public class CompanyStatisticsViewModel
    {
        public double RawMultiplier { get; set; }
        public double QualityModifier { get; set; }
        public double ProductionModifier { get; set; }
        public double DevelopmentModifier { get; set; }
        public double PeopleModifier { get; set; }

        public bool ShowRawMultiplier { get; set; }

        public bool ShowQualityModifier { get; set; }



        public CompanyStatisticsViewModel(Company company, ICompanyService companyService, IRegionService regionService, IRegionRepository regionRepository)
        {
            var calculator = new ProductionPointsCalculatorFactory()
                .SetProductType(company.GetProducedProductType())
                .Create();

            var args = new ProductionPointsCalculateArgs(company, regionService, regionRepository)
            {
                Skill = 4.0,
                Distance = 0.0,
                HitPoints = 100
            };


            RawMultiplier = Percentage.ConvertToPercent(calculator.CalculateRawMultiplier(args));
            QualityModifier = Percentage.ConvertToPercent(calculator.CalculateQualityModifier(args));
            ProductionModifier = Percentage.ConvertToPercent(calculator.CalculateProductModifier(args));
            DevelopmentModifier = Percentage.ConvertToPercent(calculator.CalculateDevelopmentModifier(args));
            PeopleModifier = Percentage.ConvertToPercent(calculator.CalculatePeopleModifier(args));

            ShowRawMultiplier = companyService.CanUseRawMultiplier(company);
            ShowQualityModifier = companyService.CanUseQualityModifier(company);
        }
    }
}