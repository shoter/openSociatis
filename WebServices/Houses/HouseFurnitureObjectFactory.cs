using Entities;
using Entities.enums;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Houses
{
    public class HouseFurnitureObjectFactory
    {
        public static IHouseFurnitureObject CreateHouseFurniture(HouseFurniture furniture)
        {
            switch (furniture.GetFurnitureType())
            {
                case FurnitureTypeEnum.Bed:
                    return new HouseBedObject(furniture);
                case FurnitureTypeEnum.Chest:
                    return new HouseChestObject(furniture);
            }

            throw new NotImplementedException();
        }
    }
}
