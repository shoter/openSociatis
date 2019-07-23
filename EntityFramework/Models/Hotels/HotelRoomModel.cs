using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Hotels
{
    public class HotelRoomModel
    {
        public long ID { get; set; }
        public string InhabitantName { get; set; }
        public string InhabitantImgUrl { get; set; }
        public int? InhabitantID { get; set; }
        public int? ToDay { get; set; }
        public int Quality { get; set; }
    }
}
