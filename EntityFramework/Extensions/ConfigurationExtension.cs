using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class ConfigurationExtension
    {
        public static decimal GetCompanyFee(this ConfigurationTable config, Entity entity)
        {
            switch(entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    {
                        return config.CompanyCitizenFee;
                    }
                case EntityTypeEnum.Organisation:
                    {
                        return config.CompanyOrganisationFee;
                    }
                case EntityTypeEnum.Country:
                    {
                        return config.CompanyCountryFee;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
