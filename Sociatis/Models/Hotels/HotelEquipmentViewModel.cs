using Entities.Models.Hotels;
using Sociatis.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Hotels
{
    public class HotelEquipmentViewModel : EquipmentViewModel
    {
        public HotelInfoViewModel Info { get; set; }

        public HotelEquipmentViewModel(HotelInfo info, Entities.Equipment equipment)
            : base(equipment)
        {
            Info = new HotelInfoViewModel(info);
        }
    }
}
