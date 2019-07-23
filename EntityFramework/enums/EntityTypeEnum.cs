using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum EntityTypeEnum
    {
        Citizen = 1,
        Country = 2,
        Organisation = 3,
        Company = 4,
        Newspaper = 5,
        Party = 6,
        Hotel = 7
    }

    public static class EntityTypeEnumExtensions
    {
        public static string ToHumanReadable(this EntityTypeEnum entityType)
        {
            switch(entityType)
            {
                case EntityTypeEnum.Citizen:
                    return "citizen";
                case EntityTypeEnum.Company:
                    return "company";
                case EntityTypeEnum.Country:
                    return "country";
                case EntityTypeEnum.Newspaper:
                    return "newspaper";
                case EntityTypeEnum.Organisation:
                    return "organisation";
                case EntityTypeEnum.Party:
                    return "party"; 
            }
#if DEBUG
            throw new NotImplementedException(string.Format("ToHumanReadable not found for {0}", entityType.ToString()));
#else
            return entityType.ToString();
#endif

        }
    }
}
