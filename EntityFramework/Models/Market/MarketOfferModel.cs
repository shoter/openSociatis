using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Market
{
    public class MarketOfferModel
    {
        public int OfferID { get; set; }
        public int Quality { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public int CurrencyID { get; set; }
        public int? CountryID { get; set; }
        public int ProductID { get; set; }
        public Entity Company { get; set; }
        public int CompanyTypeID { get; set; }

        public int CompanyRegionID { get; set; }
        public int CompanyCountryID { get; set; }
        public string CompanyCountryName { get; set; }

    }
}
