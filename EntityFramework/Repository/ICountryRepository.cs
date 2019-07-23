using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICountryRepository : IRepository<Country>
    {
        void RemoveCongressman(int citizenID, int countryID);
        void RemoveAllCongressmen(Country country);
        CountryPolicy GetCountryPolicyById(int countryID);
        double GetCountryDevelopementValue(int countryID);

        IQueryable<Country> GetNeighbourCountries(int countryID);
        IQueryable<Country> GetEnemyCountries(int countryID);
        /// <summary>
        /// Gets yours and enemies embargos
        /// </summary>
        /// <param name="countryID">country id for which we will be looking for embargoes</param>
        IQueryable<Country> GetEmbargoes(int countryID);
        List<Country> GetAllies(int countryID);
        TResult GetCountryPolicySetting<TResult>(int countryID, Func<CountryPolicy, TResult> selector);

        PresidentVoting GetLastPresidentVoting(int countryID);

    }
}
