using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using SociatisCommon.Rights;
using WebServices.Houses;

namespace Sociatis.Models.Houses
{
    public class HouseChestFurnitureViewModel : HouseBaseFurnitureViewModel
    {
        public int Capacity { get; set; }
        public int NextLevelCapacity { get; set; }

        public HouseChestFurnitureViewModel(HouseFurniture furniture, HouseRights rights) : base(furniture, rights)
        {
            Capacity = furniture.HouseChest.Capacity;
            if (CanUpgrade)
                NextLevelCapacity = HouseChestObject.GetCapacity(furniture.Quality + 1);
        }

        
    }
}
