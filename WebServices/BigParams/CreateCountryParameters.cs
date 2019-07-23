using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams
{
    public class CreateCountryParameters
    {
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyShortName { get; set; }
        public string CurrencySymbol { get; set; }
        public int CurrencyID { get; set; }

        public string Color { get; set; }
    }
}
