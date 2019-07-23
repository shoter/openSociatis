using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Calculators.ProductionPoints
{
    public class ProductionPointsCalculateArgs
    {

        public ProductionPointsCalculateArgs(Citizen citizen, Company company, IRegionService regionService, IRegionRepository regionRepository)
            :this(company, regionService, regionRepository)
        {
            Skill = citizen.GetWorkSkill(company);
            HitPoints = citizen.HitPoints;
            Distance = regionService.GetPathBetweenRegions(citizen.Region, company.Region)?.Distance ?? double.MaxValue;

        }

        public ProductionPointsCalculateArgs(Company company, IRegionService regionService, IRegionRepository regionRepository)
        {
            ProducedProduct = company.GetProducedProductType();
            Quality = company.Quality;
            Development = (double)company.Region.Development;
            PeopleCount = company.CompanyEmployees.Count;

            if (ResourceType.HasValue)
            {
                var resource = regionRepository.GetResourceForRegion(company.RegionID, ResourceType.Value);
                ResourceQuality = resource?.ResourceQuality;
            }
        }


        public ProductionPointsCalculateArgs() { }
        public ProductTypeEnum ProducedProduct { get; set; }
        public double Skill { get; set; }
        public double Distance { get; set; }
        public int HitPoints { get; set; }
        public int Quality { get; set; }
        public int? ResourceQuality { get; set; }
        public double Development { get; set; }
        public int PeopleCount { get; set; }
        public CompanyTypeEnum CompanyType => ProducedProduct.GetCompanyTypeForProduct();
        public WorkTypeEnum WorkType => ProducedProduct.GetWorkType();
        public ResourceTypeEnum? ResourceType => ProducedProduct.IsRaw() ? ProducedProduct.GetResourceType() : (ResourceTypeEnum?)null;
    }
}
