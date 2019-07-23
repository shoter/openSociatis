using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebUtils.Mvc;

namespace Sociatis.Models.Hotels
{
    public class HotelCreateRoomViewModel
    {
        public HotelInfoViewModel Info { get; set; }
        public List<SelectListItem> QualitySelect { get; set; } = new CustomSelectList().AddNumbers(1, 5);
        public int Quality { get; set; }
        public HotelCreateRoomViewModel(HotelInfo info)
        {
            Info = new HotelInfoViewModel(info);
        }
    }
}
