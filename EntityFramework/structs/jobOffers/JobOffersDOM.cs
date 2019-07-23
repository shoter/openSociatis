using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs.jobOffers
{
    public class JobOfferDOM
    {
        public int ID { get; set; }
        public JobOfferTypeEnum JobType { get; set; }
        public WorkTypeEnum WorkType { get; set; }
        public double NormalSalary { get; set; }
        public double MinimumSkill { get; set; }
        public int MinimumHP { get; set; }
        public int Length { get; set; }
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public string RegionName { get; set; }
    }
}
