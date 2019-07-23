using Entities;
using Sociatis.Controllers;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;
using WebUtils.Forms.Select2;
using WebUtils.Mvc;

namespace Sociatis.Models.Hotels
{
    public class HotelIndexViewModel
    {
        public class HotelShortViewModel
        {
            public SmallEntityAvatarViewModel Avatar { get; set; }
            public string Name { get; set; }
            public int ID { get; set; }
            public int FreeRooms { get; set; }

            public HotelShortViewModel(Hotel hotel)
            {
                ID = hotel.ID;
                Avatar = new SmallEntityAvatarViewModel(hotel.Entity)
                    .DisableNameInclude();
                Name = Avatar.Name;

                FreeRooms = hotel.HotelRooms.Count(r => r.InhabitantID.HasValue == false);
            }
        }
        public int? RegionID { get; set; }
        public CustomSelectList Regions { get; set; } = new CustomSelectList();
        public List<HotelShortViewModel> Hotels { get; set; }
        public HotelIndexViewModel(int? regionID, IEnumerable<Hotel> hotels)
        {
            RegionID = regionID;

            foreach (var r in Persistent.Regions.GetAll().OrderBy(r => r.Name))
                Regions.Add(r.ID, r.Name);

            Hotels = hotels.Select(h => new HotelShortViewModel(h))
                .OrderByDescending(h => h.FreeRooms).ToList();


        }
    }
}
