using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class EquipmentExtension
    {
        public static int GetItemCount(this Equipment equipment)
        {
            return equipment.EquipmentItems.Sum(ei => ei.Amount);
        }

        public static bool CanAddNewItem(this Equipment equipment, int amount = 1)
        {
            return equipment.GetItemCount() + amount <= equipment.ItemCapacity;
        }

        public static int GetEquipmentFreeSpaceCount(this Equipment equipment)
        {
            return equipment.ItemCapacity - equipment.GetItemCount();
        }

    }
}
