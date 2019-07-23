using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ProductCostViewModel
    {
        public decimal BasePrice { get; set; }
        public decimal? Vat { get; set; }
        public decimal? ImportTax { get; set; }
        public decimal? ExportTax { get; set; }

        public decimal ClientCost { get; set; }
        public decimal HomeCost { get; set; }
        public decimal ForeignCost { get; set; }

        public string HomeCountrySymbol { get; set; }
        public string SellingCountrySymbol { get; set; }
        public bool IsForeignVat { get; set; }

        public ProductCostViewModel(ProductCost cost, int? homeCountryID, int? sellingCountryID, int amount)
        {
            if(homeCountryID.HasValue)
            {
                HomeCountrySymbol = Persistent.Countries.GetCountryCurrency(homeCountryID.Value).Symbol;
            }
            if(sellingCountryID.HasValue)
            {
                IsForeignVat = true;
                SellingCountrySymbol = Persistent.Countries.GetCountryCurrency(sellingCountryID.Value).Symbol;
            }

            BasePrice = cost.BasePrice;
            Vat = cost.VatCost;
            ImportTax = cost.ImportTax;
            ExportTax = cost.ExportTax;

            HomeCost = cost.TotalHomeCost;
            ForeignCost = cost.TotalForeignCost;
            ClientCost = (BasePrice + (Vat ?? 0)) / amount;
        }
    }
}