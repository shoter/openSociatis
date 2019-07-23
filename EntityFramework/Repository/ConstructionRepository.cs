using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Constructions;
using Entities.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;

namespace Entities.Repository
{
    public class ConstructionRepository : RepositoryBase<Construction, SociatisEntities>, IConstructionRepository
    {
        public ConstructionRepository(SociatisEntities context) : base(context)
        {
        }

        public bool AnyConstructionTypeBuildInRegion(int regionID, ProductTypeEnum productType)
        {
            return Any(c => c.Company.RegionID == regionID && c.Company.ProductID == (int)productType);
        }

        public List<NationalConstruction> GetNationalConstructions(int countryID)
        {
            return (from company in context.Companies.Where(c => c.OwnerID == countryID)
                    join construction in Query on company.ID equals construction.ID
                    join region in context.Regions on company.RegionID equals region.ID
                    select new NationalConstruction()
                    {
                        ConstructionID = construction.ID,
                        ConstructionName = company.Entity.Name,
                        RegionID = region.ID,
                        RegionName = region.Name,
                        Progress = construction.Progress,
                        Quality  = company.Quality,
                        OwnerName = company.Owner.Name,
                        ProductTypeID = company.ProductID,

                    }).ToList();
        }
    }
}
