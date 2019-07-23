using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class HouseExtensions
    {
        public static HouseFurniture GetFurniture(this House house, FurnitureTypeEnum furnitureType)
        {
            return house.HouseFurnitures.FirstOrDefault(f => f.FurnitureTypeID == (int)furnitureType);
        }

    }
}
