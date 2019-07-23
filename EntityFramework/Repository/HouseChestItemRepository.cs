using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HouseChestItemRepository : RepositoryBase<HouseChestItem, SociatisEntities>, IHouseChestItemRepository
    {
        public HouseChestItemRepository(SociatisEntities context) : base(context)
        {
        }

        public HouseChestItem GetItem(long houseID, int productID)
        {
            return FirstOrDefault(i => i.HouseChest.HouseFurniture.HouseID == houseID &&
            i.ProductID == productID);
        }
    }
}
