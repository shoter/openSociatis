using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum JobOfferTypeEnum
    {
        Normal = 1,
        Contract = 2,

        Both = 0
    }

    public static class JobOfferTypeEnumExtensions
    {
        public static string ToHumanReadable(this JobOfferTypeEnum jobType)
        {
            switch(jobType)
            {
                case JobOfferTypeEnum.Normal:
                    return "normal";
                case JobOfferTypeEnum.Contract:
                    return "contract";
                case JobOfferTypeEnum.Both:
                    return "both";
                default:
#if DEBUG
                    throw new NotImplementedException();
#else
                    return jobType.ToString();
#endif
            }
        }
    }
}
