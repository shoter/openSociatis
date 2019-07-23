using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class Money
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }


        public Money() { }

        public Money(Currency currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public Money(int currencyID, decimal amount)
            : this(
                    currency: Persistent.Currencies.GetById(currencyID),
                    amount: amount
                 )
        { }

        public static Money operator -(Money money)
        {
            return new Money
            {
                Amount = -money.Amount,
                Currency = money.Currency
            };
        }
    }
}
