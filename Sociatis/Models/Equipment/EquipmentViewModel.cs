using Entities;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Equipment
{
    public class EquipmentViewModel
    {


        public List<EquipmentItemViewModel> Items { get; set; } = new List<EquipmentItemViewModel>();
        public int ItemCount { get; set; }
        public int ItemCapacity { get; set; }


        public EquipmentViewModel(Entities.Equipment equipment)
        {
            ItemCapacity = equipment.ItemCapacity;
            ItemCount = equipment.GetItemCount();

            var items = equipment.EquipmentItems
                .OrderBy(i => i.ProductID)
                .ThenBy(i => i.Quality)
                .ToList();
                

            foreach(var item in items)
            {
                Items.Add(
                    new EquipmentItemViewModel(item)
                    );

            }
        }

        public EquipmentViewModel(HouseChest houseChest)
        {
            ItemCapacity = houseChest.Capacity;
            ItemCount = houseChest.GetItemCount();

            var items = houseChest.HouseChestItems
                .OrderBy(i => i.ProductID)
                .ThenBy(i => i.Quality)
                .ToList();

            foreach (var item in items)
                Items.Add(new EquipmentItemViewModel(item));
        }

    }
}
