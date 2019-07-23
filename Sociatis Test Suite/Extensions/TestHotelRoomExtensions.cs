using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis_Test_Suite.Extensions
{
    public static class TestHotelRoomExtensions
    {
        public static void SetInhabitant(this HotelRoom room,Citizen citizen,  int? fromDay = null, int? toDay = null)
        {
            fromDay = fromDay ?? GameHelper.CurrentDay;
            toDay = toDay ?? (fromDay + 10);

            room.Inhabitant = citizen;
            room.InhabitantID = citizen.ID;
            room.StayingFromDay = fromDay;
            room.StayingToDay = toDay;

            citizen.HotelRooms.Add(room);

        }
    }
}
