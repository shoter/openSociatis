using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Test_Suite.Dummies
{
    public class CurrencyDummyCreator : IDummyCreator<Currency>
    {
        private Currency currency;
        private static UniqueIDGenerator idGenerator = new UniqueIDGenerator();

        public CurrencyDummyCreator()
        {
            currency = createCurrency();
        }
        public Currency Create()
        {
            var _return = currency;
            currency = createCurrency();
            Persistent.Currencies.Add(_return);
            return _return;
        }

        public CurrencyDummyCreator SetCurrency(string name, string shortName, string symbol)
        {
            currency.Symbol = symbol;
            currency.ShortName = shortName;
            currency.Name = name;

            return this;
        }

        private Currency createCurrency()
        {
            var currencyName = RandomGenerator.GenerateString(10);
            
            return new Currency()
            {
                ID = idGenerator.UniqueID,
                Name = currencyName,
                ShortName = currencyName.Substring(0, 3),
                Symbol = currencyName.Substring(0, 1)
            };
        }
    }
}
