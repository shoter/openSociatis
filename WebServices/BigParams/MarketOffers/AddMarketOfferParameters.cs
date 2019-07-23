using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.MarketOffers
{
    public class AddMarketOfferParameters
    {
        /// <summary>
        /// Amount of items to post on market 
        /// </summary>
        public int Amount { get; set; }
        public int Quality { get; set; }
        public ProductTypeEnum ProductType { get; set; }
        public int CompanyID { get; set; }
        /// <summary>
        /// Optional. Only when posting on the market
        /// </summary>
        public int? CountryID { get; set; }
        public decimal Price { get; set; }
    }
}
