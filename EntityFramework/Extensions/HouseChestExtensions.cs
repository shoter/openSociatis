using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class HouseChestExtensions
    {
        public static int GetItemCount(this HouseChest chest)
        {
            return chest.HouseChestItems
                .Sum(item => 
                item.ProductID == (int)ProductTypeEnum.UpgradePoints ? 0 : item.Quantity);
        }
    }
}
