using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum FurnitureTypeEnum
    {
        Bed = 1,
        Chest = 2
    }

    public static class FurnitureTypeEnumExtensions
    {
        public static string ToHumanReadable(this FurnitureTypeEnum furnitureType)
        {
            return furnitureType.ToString();
        }
    }
}
