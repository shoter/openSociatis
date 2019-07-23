using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum ResourceTypeEnum
    {
        Grain = 1,
        IronOre = 2,
        Oil = 3,
        TeaLeaf = 4,
        Wood = 5
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ResourceTypeEnumExtensions
    { 
        public static string ToHumanReadable(this ResourceTypeEnum resourceType)
        {
            switch(resourceType)
            {
                case ResourceTypeEnum.Grain:
                    return "Grain";
                case ResourceTypeEnum.IronOre:
                    return "Iron ore";
                case ResourceTypeEnum.Oil:
                    return "Oil";
                case ResourceTypeEnum.TeaLeaf:
                    return "Tea leaf";
                case ResourceTypeEnum.Wood:
                    return "Wood";
            }

            throw new NotImplementedException();
        }
    }
}
