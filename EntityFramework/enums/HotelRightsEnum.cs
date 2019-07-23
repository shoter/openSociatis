using Common.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum HotelRightsEnum
    {
        CanBuildRooms,
        CanMakeDeliveries,
        CanUseWallet,
        CanSetPrices,
        CanSwitchInto,
        CanManageManagers,
        CanManageEquipment
    }

    public static class HotelRightsEnumExtensions
    {
        public static string ToHumanReadable(this HotelRightsEnum hotelRights)
        {
            switch (hotelRights)
            {
                case HotelRightsEnum.CanManageEquipment:
                    return "Can manage inventory";
            }
            return StringUtils.PascalCaseToWord(hotelRights.ToString());
        }
    }
}
