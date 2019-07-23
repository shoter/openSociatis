using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs.jobOffers
{
    public class CountryBestJobOffers
    {
        public decimal? MinimumSalary { get; set; }
        public decimal? MaximumSalary { get; set; }
        public decimal? MinimumSkill { get; set; }
        public decimal? MaximumSkill { get; set; }
    }
}
