using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models
{
    public class MoneyViewModel
    {
        public int CurrencyID { get; set; }
        public string Symbol { get; set; }
        public ImageViewModel Image { get; set; }
        public decimal Quantity { get; set; }

        public MoneyViewModel()
        { }
        public MoneyViewModel(int currencyID, decimal quantity)
            : this(Persistent.Currencies.GetById(currencyID), quantity)
        { }
        public MoneyViewModel(Currency currency,decimal quantity)
        {
            CurrencyID = currency.ID;

            Symbol = currency.Symbol;
            Quantity = quantity;
            Image = Images.GetCountryCurrency(currency).VM;
        }

        public MoneyViewModel(WalletMoney vm)
            :this(Persistent.Currencies.GetById(vm.CurrencyID)
                 , vm.Amount)
        {
        }

        public MoneyViewModel(Money money) : this(money.Currency, money.Amount) { }

        public MoneyViewModel(CurrencyTypeEnum currency, decimal quantity)
            : this((int)currency, quantity)
        { }
        
        public static List<MoneyViewModel> GetMoney(Entities.Wallet wallet)
        {
            List<MoneyViewModel> money = new List<MoneyViewModel>();

            foreach(var item in wallet.WalletMoneys)
            {
                money.Add(new MoneyViewModel(item));
            }

            return money;
        }
    }
}