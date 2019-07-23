using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class OfferCost : ProductCost
    {
        public int CurrencyID { get; set; }
        public int ExportCurrencyID { get; set; }

        public decimal FuelCost { get; set; }
        public int IntegerRealCost => (int)Math.Ceiling(FuelCost);

        public void Set(ProductCost cost)
        {
            BasePrice = cost.BasePrice;
            ImportTax = cost.ImportTax;
            ExportTax = cost.ExportTax;
            VatCost = cost.VatCost;
        }

        public Money ClientPriceMoney
        {
            get
            {
                return new Money()
                {
                    Amount = (decimal)(BasePrice + VatCost),
                    Currency = Persistent.Currencies.First(c => c.ID == CurrencyID)
                };
            }
        }

        public Money ExportMoney
        {
            get
            {
                return new Money()
                {
                    Amount = (decimal)(ExportTax),
                    Currency = Persistent.Currencies.First(c => c.ID == CurrencyID)
                };
            }
        }

        public Money ImportMoney
        {
            get
            {
                return new Money
                {
                    Amount = (decimal)ImportTax,
                    Currency = Persistent.Currencies.First(c => c.ID == ExportCurrencyID)
                };
            }
        }
    }
}
