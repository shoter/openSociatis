using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IHouseChestItemRepository : IRepository<HouseChestItem>
    {
        HouseChestItem GetItem(long houseID, int productID);
    }
}
