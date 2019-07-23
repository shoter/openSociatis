using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Hotels
{
    public class HotelInfo
    {
        public string HotelName { get; set; }
        public int HotelID { get; set; }
        public decimal Condition { get; set; }
        public string ImgUrl { get; set; }
        public bool IsOwner { get; set; }
        public IEnumerable<HotelRoomInfo> HotelRoomInfos { get; set; }
        public HotelRights HotelRights { get; set; }

        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public int OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string OwnerImgUrl { get; set; }
    }
}
