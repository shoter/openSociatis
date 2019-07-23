using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class WorkStatistics
    {
        public double Production { get; set; }
        public Money Salary { get; set; }
        public Currency currency { get; set; }
    }
}
