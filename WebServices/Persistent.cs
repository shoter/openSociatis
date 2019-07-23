using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public static class Persistent
    {
        private static RegionPersistentRepository regions = null;
        private static CurrencyPersistentRepository currencies = null;
        private static CountryPersistentRepository countries = null;

        public static RegionPersistentRepository Regions
        {
            get
            {
                if (regions == null)
                    regions = new RegionPersistentRepository();
                return regions;
            }
        }
        public static CurrencyPersistentRepository Currencies
        {
            get
            {
                if (currencies == null)
                    currencies = new CurrencyPersistentRepository();
                return currencies;
            }
        }
        public static CountryPersistentRepository Countries
        {
            get
            {
                if (countries == null)
                    countries = new CountryPersistentRepository();
                return countries;
            }
        }
        public static void Init()
        {
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
#endif
            regions = new RegionPersistentRepository();
            currencies = new CurrencyPersistentRepository();
            countries = new CountryPersistentRepository();

#if DEBUG
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine(string.Format("Persistent load time : {0} ms", stopwatch.ElapsedMilliseconds));
#endif
        }

        public static void Init(RegionPersistentRepository regionPersistentRepository, CurrencyPersistentRepository currencyPersistentRepository,
            CountryPersistentRepository countryPersistentRepository)
        {
            regions = regionPersistentRepository;
            currencies = currencyPersistentRepository;
            countries = countryPersistentRepository;
        }

    }
}
