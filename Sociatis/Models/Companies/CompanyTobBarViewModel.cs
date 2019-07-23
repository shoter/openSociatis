using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Companies
{
    public class CompanyTobBarViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public int CompanyID { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public bool IsWorkingHere { get; set; }
    }
}
