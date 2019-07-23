using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using Common.EntityFramework;

namespace Entities.Repository
{
    public class CountryRepository : RepositoryBase<Country, SociatisEntities>, ICountryRepository
    {
        public CountryRepository(SociatisEntities context) : base(context)
        {
        }

        public override IQueryable<Country> Query
        {
            get
            {
                return base.Query.Include(c => c.Entity);
            }
        }

        public void RemoveCongressman(int citizenID, int countryID)
        {
            var congressman = context.Congressmen.First(c => c.CountryID == countryID && c.CitizenID == citizenID);
            RemoveSpecific(congressman);
        }

        public void RemoveAllCongressmen(Country country)
        {
            country.Congressmen.Clear();
        }

        public double GetCountryDevelopementValue(int countryID)
        {
            return (double)context
                .Regions.Where(r => r.CountryID == countryID)
                .Select(r => r.Development)
                .Sum();
        }



        public CountryPolicy GetCountryPolicyById(int countryID)
        {
            return context.CountryPolicies
                .FirstOrDefault(cp => cp.CountryID == countryID);
        }

        public IQueryable<Country> GetNeighbourCountries(int countryID)
        {
            var countryRegions = context.Regions.Where(r => r.CountryID == countryID);

            var neighbours = countryRegions
                .SelectMany(r => r.Passages.Select(p => p.FirstRegion.Country));

            neighbours = neighbours.Union(countryRegions
                .SelectMany(r => r.Passages.Select(p => p.SecondRegion.Country)));

            neighbours = neighbours.Union(countryRegions
                .SelectMany(r => r.Passages1.Select(p => p.FirstRegion.Country)));

            neighbours = neighbours.Union(countryRegions
                .SelectMany(r => r.Passages1.Select(p => p.SecondRegion.Country)));

            neighbours = neighbours.Distinct();

            var test = neighbours.ToList();

            neighbours = neighbours.Where(n => n.ID != countryID);

            return neighbours;
        }

        public List<Country>  GetAllies(int countryID)
        {
            var allies = context.MilitaryProtectionPacts.Where(mpp => mpp.FirstCountryID == countryID)
                .Where(mpp => mpp.Active)
                .Select(mpp => mpp.SecondCountry);

            allies = allies.Union(context.MilitaryProtectionPacts.Where(mpp => mpp.SecondCountryID == countryID)
                .Where(mpp => mpp.Active)
                .Select(mpp => mpp.FirstCountry));


            return allies.ToList();
        }

        public IQueryable<Country> GetEnemyCountries(int countryID)
        {
            var attackerWars = context.Wars
                .Where(w => w.Active)
                .Where(w => w.AttackerCountryID == countryID || w.CountryInWars.Any(ciw => ciw.IsAttacker && ciw.CountryID == countryID));

            var defenderWars = context.Wars
                .Where(w => w.Active)
                .Where(w => w.DefenderCountryID == countryID || w.CountryInWars.Any(ciw => ciw.IsAttacker == false && ciw.CountryID == countryID));

            var countriesAtWar =
                 attackerWars
                .SelectMany(w => w.CountryInWars.Where(ciw => ciw.IsAttacker == false && ciw.CountryID != countryID).Select(ciw => ciw.Country))
                .Union(attackerWars.Select(w => w.Defender))

                .Union(defenderWars
                    .SelectMany(w => w.CountryInWars.Where(ciw => ciw.IsAttacker == true && ciw.CountryID != countryID).Select(ciw => ciw.Country))
                    )
                .Union(defenderWars.Select(w => w.Attacker))
                .Distinct();

            return countriesAtWar;
        }

        /// <summary>
        /// Returns countries that were embargoed by specified country
        /// </summary>
        /// <param name="countryID">Country wich created embargoes</param>
        public IQueryable<Country> GetEmbargoes(int countryID)
        {
            return context.Embargoes
                .Where(e => e.Active && e.CreatorCountryID == countryID)
                .Select(e => e.EmbargoedCountry);
        }

        public TResult GetCountryPolicySetting<TResult>(int countryID, Func<CountryPolicy, TResult> selector)
        {
            return Where(c => c.ID == countryID)
                .Select(c => c.CountryPolicy)
                .Select(selector)
                .First();
        }

        public PresidentVoting GetLastPresidentVoting(int countryID)
        {
            return context.PresidentVotings
                .Where(pv => pv.CountryID == countryID)
                .OrderByDescending(pv => pv.StartDay)
                .First();
        }
    }
}
