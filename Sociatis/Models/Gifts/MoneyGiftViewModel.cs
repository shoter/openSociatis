using Entities;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Gifts
{
    public class MoneyGiftViewModel
    {
        public ImageViewModel MoneyImage { get; set; }
        public double Amount { get; set; }
        public string Symbol { get; set; }
        public int CurrencyID { get; set; }

        public MoneyGiftViewModel(WalletMoney money)
        {
            var currency = Persistent.Currencies.GetById(money.CurrencyID);
            MoneyImage = Images.GetCountryCurrency(currency).VM;
            Amount = (double)money.Amount;
            Symbol = currency.Symbol;
            CurrencyID = currency.ID;
        }
    }
}