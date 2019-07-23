using Entities;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Houses
{
    public class HouseFurnitureListViewModel
    {
        public HouseInfoViewModel Info { get; set; }
        public List<HouseBaseFurnitureViewModel> Furnitures { get; set; } = new List<HouseBaseFurnitureViewModel>();
        public HouseFurnitureListViewModel(House house, IEnumerable<HouseFurniture> furniture, HouseRights houseRights)
        {
            Info = new HouseInfoViewModel(house, houseRights);

            foreach (var f in furniture)
            {
                Furnitures.Add(HouseFurnitureViewModelFactory.Create(f, houseRights));
            }

        }
    }
}
