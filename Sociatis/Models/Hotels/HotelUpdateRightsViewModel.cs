using Common;
using Entities.enums;
using Entities.Models.Hotels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Hotels
{
    public class HotelUpdateRightsViewModel
    {
        public int RightID { get; set; }
        public bool Value { get; set; }

        public HotelRightsEnum HotelRights => (HotelRightsEnum)RightID;

    }
}
