using Entities.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Events
{
    public class CountryEventViewModel : EventViewModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public CountryEventViewModel(CountryEventModel e) : base(e)
        {
            CountryID = e.CountryID;
            CountryName = e.CountryName;
        }
        
    }
}
