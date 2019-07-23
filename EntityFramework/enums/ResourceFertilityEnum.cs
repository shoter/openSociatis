using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum ResourceFertilityEnum
    {
        None = 0,
        Scarce = 1,
        Normal = 2,
        Good = 3,
        Abundant = 4
    }

    public static class ResourceFertilityEnumExtensions
    {
        public static string ToHumanReadable(this ResourceFertilityEnum fertility)
        {
            switch(fertility)
            {
                case ResourceFertilityEnum.None:
                    return "none";
                case ResourceFertilityEnum.Scarce:
                    return "scarce";
                case ResourceFertilityEnum.Normal:
                    return "normal";
                case ResourceFertilityEnum.Good:
                    return "good";
                case ResourceFertilityEnum.Abundant:
                    return "abundant";
            }
#if DEBUG
            throw new NotImplementedException("Fertility not detected - " + fertility.ToString());
#else
            return fertility.ToString();
#endif
        }
    }
}
