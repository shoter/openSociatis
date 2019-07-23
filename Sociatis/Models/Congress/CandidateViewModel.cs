using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress
{
    public class CandidateViewModel
    {
        public CongressInfoViewModel Info { get; set; }
        

        public CandidateViewModel(Entities.Country country)
        {
            Info = new CongressInfoViewModel(country);
        }
    }
}