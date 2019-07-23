using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class HouseFurnitureExtensions
    {
        public static FurnitureTypeEnum GetFurnitureType(this HouseFurniture furniture)
        {
            return (FurnitureTypeEnum)furniture.FurnitureTypeID;
        }
    }
}
