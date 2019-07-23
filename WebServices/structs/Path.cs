using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class Path
    {
        public Region StartRegion { get; set; }
        public Region EndRegion { get; set; }
        public double Distance { get; set; }

    }
}
