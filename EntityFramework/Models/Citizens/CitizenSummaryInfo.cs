using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Citizens
{
    public class CitizenSummaryInfo
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public bool CanWork { get; set; }
        public int HitPoints { get; set; }
        public int Experience { get; set; }
        public bool Trained { get; set; }
        public double Strength { get; set; }
        public double MilitaryRank { get; set; }
        public string AvatarUrl { get; set; }
        public int? JobID { get; set; }
        public bool EatingSafety { get; set; }
        public double GoldAmount { get; set; }
        public double CountryMoneyAmount { get; set; }
        public int CountryCurrencyID { get; set; }
        public int CountryID { get; set; }
        public int Level { get; set; }
        public bool CanHeal { get; set; }
        public decimal? HealingCost { get; set; }
        public int? HospitalID { get; set; }

        public int UnreadMessages { get; set; }
        public int UnreadWarnings { get; set; }
    }
}
