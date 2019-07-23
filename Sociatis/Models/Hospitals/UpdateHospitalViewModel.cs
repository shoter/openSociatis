using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Hospitals
{
    public class UpdateHospitalViewModel
    {
        public int HospitalID { get; set; }
        public decimal? HealingPrice { get; set; }
        public bool HealingEnabled { get; set; }
        public bool HealingFree { get; set; }

    }
}