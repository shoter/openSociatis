using Common.EntityFramework;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IHouseFurnitureRepository : IRepository<HouseFurniture>
    {
        HouseChest GetHouseChest(long houseID);
        HouseFurniture GetFurniture(long houseID, FurnitureTypeEnum furnitureType);
        IEnumerable<HouseFurniture> GetFurniture(long houseID);
        IEnumerable<FurnitureTypeEnum> GetUnbuiltFurniture(long houseID);
        bool IsFurnitureBuilt(long houseID, FurnitureTypeEnum furnitureType);
    }
}
