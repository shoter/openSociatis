using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class ProductCost
    {
        public decimal BasePrice { get; set; }
        public decimal? VatCost { get; set; }
        public decimal? ImportTax { get; set; }
        public decimal? ExportTax { get; set; }

        public decimal TotalForeignCost
        {
            get
            {
                decimal cost = ExportTax ?? 0;
                return cost;
            }
        }

        public decimal TotalHomeCost
        {
            get
            {
                decimal cost = (ImportTax ?? 0);
                return cost;
            }
        }

        public decimal CustomerCost => BasePrice + (VatCost ?? 0m);
    }
}
