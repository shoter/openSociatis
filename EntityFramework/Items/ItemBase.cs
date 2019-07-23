using Entities.enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Items
{
    public abstract class ItemBase
    {
        public abstract ProductTypeEnum ProductType { get; }

        public int Quantity { get; set; }
        public int Quality { get; set; }

        public ItemBase(int quantity, int quality)
        {
            Quantity = quantity;
            Quality = quality;
        }

        public ItemBase(EquipmentItem item)
        {
            Debug.Assert(item.ProductID == (int)ProductType);

            Quantity = item.Amount;
            Quality = item.Quality;
        }
    }
}
