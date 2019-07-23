using Entities;
using Entities.enums;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Houses
{
    public class HouseFurnitureViewModelFactory
    {
        public static HouseBaseFurnitureViewModel Create(HouseFurniture furniture, HouseRights rights)
        {
            switch ((FurnitureTypeEnum)furniture.FurnitureTypeID)
            {
                case FurnitureTypeEnum.Bed:
                    return new HouseBedFurnitureViewModel(furniture, rights);
                case FurnitureTypeEnum.Chest:
                    return new HouseChestFurnitureViewModel(furniture, rights);
            }
            throw new ArgumentException();
        }
    }
}
