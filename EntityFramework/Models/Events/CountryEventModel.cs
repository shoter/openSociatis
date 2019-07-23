using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Events
{
    public class CountryEventModel : EventModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }

    }
}
