using Entities;
using Entities.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSociatisWorld.Creators
{
    class CountryCreator
    {
        /*
        ICountryRepository countryRepository;
        ICurrencyRepository currencyRepository;

        EntityCreator entityCreator = new EntityCreator();

        Country country;

        public CountryCreator()
        {
            countryRepository = Ninject.Current.Get<ICountryRepository>();
            currencyRepository = Ninject.Current.Get<ICurrencyRepository>();
        }

        public CountryCreator Set(string name)
        {
            country.Entity.Name = name;

            return this;
        }

        public CountryCreator SetCurrency(string name, string shortName, string symbol)
        {
            var exist = currencyRepository.Any(c => c.Name == name && c.ShortName == shortName && c.Symbol == symbol);

            if (exist)
                return this;

            var currency = new Currency()
            {
                Name = name,
                ShortName = shortName,
                Symbol = symbol
            };

            country.Currency = currency;

            return this;
        }

        public Country Create()
        {

        }

        private void create()
        {
            country = new Country();

            country.Entity = entityCreator.Get();
            country.Entity.Country = country;
        }
        */

    }
}
