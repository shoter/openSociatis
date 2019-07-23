using Common.Extensions;
using Entities;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Houses;

namespace Sociatis.Models.Houses
{
    public class HouseBaseFurnitureViewModel
    {
        public string Name { get; set; }
        public int Quality { get; set; }
        public decimal MaintainceCost { get; set; }
        public bool CanUpgrade { get; set; }
        public int UpgradeCost { get; set; }
        public long HouseID { get; set; }
        public int FurnitureTypeID { get; set; }
        public bool CanManage { get; set; }

        public HouseBaseFurnitureViewModel(HouseFurniture furniture, HouseRights houseRights)
        {
            var furnitureObject = HouseFurnitureObjectFactory.CreateHouseFurniture(furniture);
            Quality = furniture.Quality;
            MaintainceCost = furnitureObject.CalculateDecay();
            Name = furnitureObject.ToHumanReadable().FirstUpper();
            HouseID = furniture.HouseID;
            FurnitureTypeID = furniture.FurnitureTypeID;
            CanUpgrade = furnitureObject.CanUpgrade() && houseRights.CanModifyHouse;
            CanManage = houseRights.CanModifyHouse;
            if(CanUpgrade)
            UpgradeCost = furnitureObject.GetUpgradeCost();
        }
    }
}
