using Common.Extensions;
using Entities;
using Entities.enums;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Houses;

namespace Sociatis.Models.Houses
{
    public class HouseCreateFurnitureViewModel
    {
        public class FurnitureForCreateViewModel
        {
            public int FurnitureTypeID { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
        }
        public HouseInfoViewModel Info { get; set; }
        public List<FurnitureForCreateViewModel> FurnitureForCreate { get; set; } = new List<FurnitureForCreateViewModel>();

        public HouseCreateFurnitureViewModel(House house, HouseRights rights, IEnumerable<FurnitureTypeEnum> unbuiltFurniture)
        {
            Info = new HouseInfoViewModel(house, rights);

            foreach (var f in unbuiltFurniture)
                FurnitureForCreate.Add(GetFurnitureForCreate(f));
        }


        public FurnitureForCreateViewModel GetFurnitureForCreate(FurnitureTypeEnum furnitureType)
        {
            return new FurnitureForCreateViewModel()
            {
                Name = furnitureType.ToHumanReadable().FirstUpper(),
                FurnitureTypeID = (int)furnitureType,
                Price = HouseFurnitureObject.GetUpgradeCost(furnitureType, 1)
            };
        }
    }
}
