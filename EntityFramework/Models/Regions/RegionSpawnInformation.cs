using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Regions
{
    public class RegionSpawnInformation
    {
        public string RegionName { get; set; }
        public int RegionID { get; set; }
        public bool SpawnEnabled { get; set; }
    }
}
