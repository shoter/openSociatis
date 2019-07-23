using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams
{
    public class CreateRegionParameters
    {
        public string Name { get; set; }
        public int CountryID { get; set; }
        public bool CanSpawn { get; set; }
    }
}
