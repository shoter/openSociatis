using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum WorkTypeEnum
    {
        Raw = 1,
        Manufacturing = 2,
        Construction = 3,
        Selling = 4,

        Any = 0
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]

    public static class WorkTypeEnumExtensions
    {
        public static string ToHumanReadable(this WorkTypeEnum workType)
        {
            switch(workType)
            {
                case WorkTypeEnum.Any:
                    return "Any";
                case WorkTypeEnum.Construction:
                    return "construction";
                case WorkTypeEnum.Manufacturing:
                    return "manufacturing";
                case WorkTypeEnum.Raw:
                    return "raw";
                case WorkTypeEnum.Selling:
                    return "selling";
            }
#if DEBUG
            throw new ArgumentException(string.Format("{0} not human readable", workType.ToString()));
#else
            return workType.ToString();
#endif
        }
    }
}
