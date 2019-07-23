using Entities;
using Entities.Models.Hotels;
using Sociatis.Helpers;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Hotels
{
    public class HotelRoomsViewModel
    {
        public class HotelRoomViewModel
        {
            public int? InhabitantID { get; set; }
            public SmallEntityAvatarViewModel Avatar { get; set; }
            public string InhabitantName { get; set; }
            public int? InhabitantLastDay { get; set; }
            public int Quality { get; set; }
            public long ID { get; set; }

            public HotelRoomViewModel(HotelRoomModel room)
            {
                InhabitantID = room.InhabitantID;
                InhabitantName = room.InhabitantName;
                InhabitantLastDay = room.ToDay;
                Quality = room.Quality;
                ID = room.ID;
                Avatar = new SmallEntityAvatarViewModel(room.InhabitantID, "", room.InhabitantImgUrl)
                    .DisableNameInclude();

            }
        }

        public Dictionary<int, List<HotelRoomViewModel>> Rooms { get; set; }
        public HotelInfoViewModel Info { get; set; }

        public HotelRoomsViewModel(HotelInfo info, IEnumerable<HotelRoomModel> hotelRooms)
        {
            Info = new HotelInfoViewModel(info);

            Rooms = hotelRooms
                .Select(r => new HotelRoomViewModel(r))
                .GroupBy(r => r.Quality)
                .OrderBy(r => r.Key)
                .ToDictionary(r => r.Key, r => r.ToList());
        }
    }


}