using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CountryExtensions
    {
        public static CongressCandidateVoting GetLastCongressCandidateVoting(this Country country)
        {
            return country.CongressCandidateVotings
                .OrderByDescending(cc => cc.ID)
                .First();
        }

        public static List<Country> GetAllies(this Country country)
        {
            List<Country> countries = country.MPPsFirstCountry
                .Where(mpp => mpp.Active)
                .Select(mpp => mpp.FirstCountry)
                .ToList();

            countries.AddRange(country.MPPsSecondCountry
                .Where(mpp => mpp.Active)
                .Select(mpp => mpp.SecondCountry)
                .ToList());

            return countries;
        }

        public static List<Country> GetTruceCountries(this Country country)
        {
            List<Country> countries = country.Truces
               .Where(mpp => mpp.Active)
               .Select(mpp => mpp.Country)
               .ToList();

            countries.AddRange(country.Truces1
                .Where(mpp => mpp.Active)
                .Select(mpp => mpp.Country1)
                .ToList());

            return countries;
        }
    }
}
