using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.MonetaryMarket
{
    public class MyOfferViewModel
    {
        public MonetaryOfferTypeEnum OfferType { get; set; }
        public string SellCurrency { get; set; }
        public string BuyCurrency { get; set; }
        public int Amount { get; set; }
        public double Rate { get; set; }
        public double ReservedMoney { get; set; }
        public string SellerCurrency { get; set; }
        public int OfferID { get; set; }

        public MyOfferViewModel(MonetaryOffer offer)
        {
            OfferID = offer.ID;
            OfferType = (MonetaryOfferTypeEnum)offer.OfferTypeID;
            SellCurrency = getSymbol(offer.SellCurrencyID);
            BuyCurrency = getSymbol(offer.BuyCurrencyID);
            Amount = offer.Amount;
            Rate = (double)offer.Rate;
            ReservedMoney = (double)(offer.OfferReservedMoney + offer.TaxReservedMoney);

            SellerCurrency = OfferType == MonetaryOfferTypeEnum.Buy 
                ? SellCurrency
                : BuyCurrency;
        }

        private string getSymbol(int currencyID)
        {
           return Persistent.Currencies.GetById(currencyID).Symbol;
        }
    }
}