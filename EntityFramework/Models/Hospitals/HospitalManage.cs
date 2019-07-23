using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Hospitals
{
    public class HospitalManage
    {
        public bool HealingEnabled { get; set; }
        public decimal? HealingPrice { get; set; }
        public int CurrencyID { get; set; }
        public List<HospitalManageNationalityHealingOption> hospitalManageNationalityHealingOptions { get; set; }
    }

    public class HospitalManageNationalityHealingOption
    {
        public decimal? HealingPrice { get; set; }
        public int CountryID { get; set; }
    }
}
