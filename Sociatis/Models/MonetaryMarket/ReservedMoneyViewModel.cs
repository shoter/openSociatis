using Entities;
using Entities.structs.MonetaryMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.MonetaryMarket
{
    public class ReservedMoneyViewModel
    {
        public string CurrencySymbol { get; set; }
        public double OfferReserved { get; set; }
        public double TaxReserved { get; set; }
        public double Sum { get { return OfferReserved + TaxReserved; } }

        public ReservedMoneyViewModel(ReservedMoney reserved, Currency currency)
        {
            CurrencySymbol = currency.Symbol;
            TaxReserved = reserved.TaxReserved;
            OfferReserved = reserved.OfferReserved;
        }
    }
}