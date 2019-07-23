using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Hotels
{
    public class HotelRights
    {
        public bool this[HotelRightsEnum right]
        {
            get
            {
                switch (right)
                {
                    case HotelRightsEnum.CanBuildRooms:
                        return CanBuildRooms;
                    case HotelRightsEnum.CanMakeDeliveries:
                        return CanMakeDeliveries;
                    case HotelRightsEnum.CanManageEquipment:
                        return CanManageEquipment;
                    case HotelRightsEnum.CanManageManagers:
                        return CanManageManagers;
                    case HotelRightsEnum.CanSetPrices:
                        return CanSetPrices;
                    case HotelRightsEnum.CanSwitchInto:
                        return CanSwitchInto;
                    case HotelRightsEnum.CanUseWallet:
                        return CanUseWallet;
                }
                throw new NotImplementedException();
            }
            set
            {
                switch (right)
                {
                    case HotelRightsEnum.CanBuildRooms:
                        CanBuildRooms = value;
                        return;
                    case HotelRightsEnum.CanMakeDeliveries:
                        CanMakeDeliveries = value;
                        return;
                    case HotelRightsEnum.CanManageEquipment:
                        CanManageEquipment = value;
                        return;
                    case HotelRightsEnum.CanManageManagers:
                        CanManageManagers = value;
                        return;
                    case HotelRightsEnum.CanSetPrices:
                        CanSetPrices = value;
                        return;
                    case HotelRightsEnum.CanSwitchInto:
                        CanSwitchInto = value;
                        return;
                    case HotelRightsEnum.CanUseWallet:
                        CanUseWallet = value;
                        return;
                }
                throw new NotImplementedException();
            }
        }
        public bool CanBuildRooms { get; set; }
        public bool CanMakeDeliveries { get; set; }
        public bool CanUseWallet { get; set; }
        public bool CanSetPrices { get; set; }
        public bool CanSwitchInto { get; set; }
        public bool CanManageManagers { get; set; }
        public bool CanManageEquipment { get; set; }

        public int Priority { get; set; }

        public bool AnyRights =>
            CanBuildRooms || CanMakeDeliveries ||
            CanUseWallet || CanSetPrices ||
            CanSwitchInto || CanManageManagers ||
            CanManageEquipment;
        public bool AllRights =>
            CanBuildRooms && CanMakeDeliveries &&
            CanUseWallet && CanSetPrices &&
            CanSwitchInto && CanManageManagers &&
            CanManageEquipment;

        public static readonly HotelRights NoRights = new HotelRights() { Priority = int.MinValue};

        public static readonly HotelRights FullRights = new HotelRights()
        {
            CanBuildRooms = true,
            CanMakeDeliveries = true,
            CanSetPrices = true,
            CanUseWallet = true,
            CanSwitchInto = true,
            CanManageManagers = true,
            CanManageEquipment = true,
            Priority = int.MaxValue
            
        };

        public HotelRights() { }

        public HotelRights(HotelManager manager)
        {
            CanBuildRooms = manager.CanBuildRooms;
            CanMakeDeliveries = manager.CanMakeDeliveries;
            CanUseWallet = manager.CanUseWallet;
            CanSetPrices = manager.CanSetPrices;
            CanSwitchInto = manager.CanSwitchInto;
            CanManageManagers = manager.CanManageManagers;
            Priority = manager.Priority;
            CanManageEquipment = manager.CanManageEquipment;
        }

        public void Update(HotelManager manager)
        {
            manager.CanBuildRooms = CanBuildRooms;
            manager.CanMakeDeliveries = CanMakeDeliveries;
            manager.CanUseWallet = CanUseWallet;
            manager.CanSetPrices = CanSetPrices;
            manager.CanSwitchInto = CanSwitchInto;
            manager.CanManageManagers = CanManageManagers;
            manager.Priority = Priority;
            manager.CanManageEquipment = CanManageEquipment;
        }
    }
}
