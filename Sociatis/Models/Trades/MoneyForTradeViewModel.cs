using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Trades
{
    public class MoneyForTradeViewModel : ItemForTradeViewModel
    {
        public int CurrencyID { get; set; }
        public ImageViewModel Image { get; set; }
        public string Symbol { get; set; }
        public decimal Amount { get; set; }

        public MoneyForTradeViewModel(TradeMoney money, TradeStatusEnum tradeStatus) :base(money.EntityID, money.DateAdded, money.TradeID, tradeStatus)
        {
            CurrencyID = money.CurrencyID;
            var currency = Persistent.Currencies.GetById(money.CurrencyID);
            Image = Images.GetCountryCurrency(currency).VM;
            Symbol = currency.Symbol;
            Amount = money.Amount;
        }
    }
}