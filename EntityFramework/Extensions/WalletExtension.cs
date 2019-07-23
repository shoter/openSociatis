using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class WalletExtension
    {
        public static WalletMoney GetMoney(this Wallet wallet, CurrencyTypeEnum currencyType, IEnumerable<Currency> currencies)
        {
            return wallet.GetMoney((int)currencyType, currencies);
        }

        public static WalletMoney GetMoney(this Wallet wallet, int currencyID, IEnumerable<Currency> currencies)
        {
            var money = wallet.WalletMoneys.FirstOrDefault(wm => wm.CurrencyID == currencyID);
            if(money == null)
            {
                return new WalletMoney()
                {
                    Amount = 0,
                    Currency = currencies.First(c => c.ID == currencyID),
                    CurrencyID = currencyID,
                    Wallet = wallet,
                    WalletID = wallet.ID
                };
            }
            return money;
        }

        public static bool HaveMoney(this Wallet wallet, int currencyID, double amount)
        {
            if (amount <= 0)
                return true;
            var money = wallet.WalletMoneys.FirstOrDefault(wm => wm.CurrencyID == currencyID);

            if (money == null)
                return false;
            return (double)money.Amount >= amount;
        }
    }
}
