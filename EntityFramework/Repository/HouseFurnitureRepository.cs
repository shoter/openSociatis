using Common;
using Common.EntityFramework;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HouseFurnitureRepository : RepositoryBase<HouseFurniture, SociatisEntities>, IHouseFurnitureRepository
    {
        public HouseFurnitureRepository(SociatisEntities context) : base(context)
        {
        }

        public HouseChest GetHouseChest(long houseID)
        {
            return (from furniture in Where(f => f.HouseID == houseID && f.FurnitureTypeID == (int)FurnitureTypeEnum.Chest)
                    join chest in context.HouseChests on furniture.ID equals chest.FurnitureID
                    select chest).FirstOrDefault();
        }

        public HouseFurniture GetFurniture(long houseID, FurnitureTypeEnum furnitureType)
        {
            return FirstOrDefault(f => f.HouseID == houseID && f.FurnitureTypeID == (int)furnitureType);
        }

        public bool IsFurnitureBuilt(long houseID, FurnitureTypeEnum furnitureType)
        {
            return Any(f => f.HouseID == houseID && f.FurnitureTypeID == (int)furnitureType);
        }
        public IEnumerable<HouseFurniture> GetFurniture(long houseID)
        {
            return Where(f => f.HouseID == houseID)
                .Include(f => f.HouseChest)
                .ToList();
        }

        public IEnumerable<FurnitureTypeEnum> GetUnbuiltFurniture(long houseID)
        {
            var furnitureTypes = Enums.ToArray<FurnitureTypeEnum>();

            var builtFurnitures = Where(f => f.HouseID == houseID)
                .Select(f => f.FurnitureTypeID).ToList();

            return furnitureTypes
                .Where(f => builtFurnitures.Contains((int)f) == false);
        }
    }
}
