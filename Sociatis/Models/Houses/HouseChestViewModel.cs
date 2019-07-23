using Entities;
using Sociatis.Models.Equipment;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Houses
{
    public class HouseChestViewModel
    {
        public HouseInfoViewModel Info { get; set; }
        public EquipmentViewModel CitizenEquipment { get; set; }
        public EquipmentViewModel ChestEquipment { get; set; }

        public HouseChestViewModel(House house, Entities.Equipment citizenEquipment, HouseChest chest, HouseRights houseRights)
        {
            Info = new HouseInfoViewModel(house, houseRights);
            CitizenEquipment = new EquipmentViewModel(citizenEquipment);
            ChestEquipment = new EquipmentViewModel(chest);
        }
    }
}
