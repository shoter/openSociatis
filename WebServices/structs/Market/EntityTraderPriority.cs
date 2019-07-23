using Entities;
using Entities.enums;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Market
{
    public static class EntityTraderPriority
    {
        public static int GetPriority(Entity entity)
        {
            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    return 4;
                case EntityTypeEnum.Hotel:
                case EntityTypeEnum.Newspaper:
                    return 2;
                case EntityTypeEnum.Company:
                    return GetPriority(entity.Company);
            }
            throw new NotImplementedException();
        }

        public static int GetPriority(Company company)
        {
            switch (company.GetCompanyType())
            {
                case CompanyTypeEnum.Producer:
                    return 1;
                case CompanyTypeEnum.Manufacturer:
                case CompanyTypeEnum.Developmenter:
                case CompanyTypeEnum.Construction:
                    return 2;
                
                
                case CompanyTypeEnum.Shop:
                    return 3;
            }
            throw new NotImplementedException();
        }
    }
}
