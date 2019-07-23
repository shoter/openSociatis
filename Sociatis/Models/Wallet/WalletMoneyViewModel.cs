using Entities;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Wallet
{
    public class WalletMoneyViewModel
    {
        public double Amount { get; set; }
        public ImageViewModel CurrencyImage { get; set; }
        public WalletMoneyViewModel(WalletMoney money)
        {
            Amount = (double)money.Amount;
            CurrencyImage = Images.GetCountryCurrency(Persistent.Currencies.GetById(money.CurrencyID)).VM;
        }
    }
}