using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Equipment
{
    public class InventoryViewModel
    {
        public Entities.Equipment Equipment { get; set; }
        public int ItemCount { get; set; }
        public int ItemCapacity { get; set; }

        public InventoryViewModel() { }
        public InventoryViewModel(Entities.Equipment equipment)
        {
            ItemCapacity = equipment.ItemCapacity;
            ItemCount = equipment.GetItemCount();

            Equipment = equipment;
        }
    }
}