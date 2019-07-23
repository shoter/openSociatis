using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CountryPolicyExtension
    {
        public static decimal GetCompanyCost(this CountryPolicy policy, Entity entity)
        {
            switch(entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    {
                        return policy.CitizenCompanyCost;
                    }
                case EntityTypeEnum.Organisation:
                    {
                        return policy.OrganisationCompanyCost;
                    }
                case EntityTypeEnum.Country:
                    {
                        return 0m;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
