using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CountryPersistentRepository : PersistentRepositoryBase<Country>, ICountryRepository
    {
        public CountryPersistentRepository() : base(new PersistentListStorage<Country>())
        {
        }

        public CountryPolicy GetCountryPolicyById(int countryID)
        {
            throw new NotImplementedException();
        }

        public double GetCountryDevelopementValue(int countryID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Country> GetEmbargoes(int countryID)
        {
            throw new NotImplementedException();
        }

        public IList<Country> GetEnemyCountries(int countryID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Country> GetNeighbourCountries(int countryID)
        {
            throw new NotImplementedException();
        }

        protected override void AfterLoad()
        {
           
        }

        protected override void LoadEntity(Country persistentEntity, Country dbEntity)
        {
            Load(x => x.Currency, persistentEntity, dbEntity);
            Load(x => x.Entity, persistentEntity, dbEntity);
        }

        public Currency GetCountryCurrency(int countryID)
        {
            return
                First(c => c.ID == countryID)
                .Currency;
        }

        public Currency GetCountryCurrency(Country country)
        {
            return GetCountryCurrency(country.ID);
        }

        IQueryable<Country> ICountryRepository.GetEnemyCountries(int countryID)
        {
            throw new NotImplementedException();
        }

        public override Country GetById(int id)
        {
            return FirstOrDefault(c => c.ID == id);
        }

        public List<Country> GetAllies(int countryID)
        {
            throw new NotImplementedException();
        }

        public TResult GetCountryPolicySetting<TResult>(int countryID, Func<CountryPolicy, TResult> selector)
        {
            throw new NotImplementedException();
        }

        public PresidentVoting GetLastPresidentVoting(int countryID)
        {
            throw new NotImplementedException();
        }

        public void RemoveCongressman(int citizenID, int countryID)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllCongressmen(Country country)
        {
            throw new NotImplementedException();
        }
    }
}
