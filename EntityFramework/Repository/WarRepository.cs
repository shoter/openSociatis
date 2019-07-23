using Common.EntityFramework;
using Entities.QueryEnums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class WarRepository : RepositoryBase<War, SociatisEntities>, IWarRepository
    {
        public WarRepository(SociatisEntities context) : base(context)
        {
        }

        public List<War> GetAllActiveWars()
        {
            return Where(w => w.Active && w.IsTrainingWar == false).ToList();
        }

        public Battle GetTrainingBattle()
        {
            return Where(w => w.IsTrainingWar)
                .Select(w => w.Battles.FirstOrDefault())
                .FirstOrDefault();
        }

        public IQueryable<CountryInWar> GetCountriesInWar(int warID)
        {
            return context.Wars
                .Where(w => w.ID == warID)
                .SelectMany(w => w.CountryInWars);
        }
        public IQueryable<War> GetDirectWarsForCountry(int countryID, WarActivitySearchCriteria criteria = WarActivitySearchCriteria.Active)
        {
            var query = context.Wars
                .Where(w => w.IsTrainingWar == false && (w.AttackerCountryID == countryID || w.DefenderCountryID == countryID));

            query = applyActivityCriteria(criteria, query);

            return query;
        }

        public IQueryable<Battle> GetActiveBattles(int countryID)
        {
            return context.Battles
                .Where(b => b.Active && b.War.IsTrainingWar == false)
                .Where(b
                => b.War.AttackerCountryID == countryID
                || b.War.DefenderCountryID == countryID
                || b.War.CountryInWars.Any(ciw => ciw.CountryID == countryID)
                );
        }
        public IQueryable<War> GetIndirectWarsForCountry(int countryID, WarActivitySearchCriteria criteria = WarActivitySearchCriteria.Active)
        {
            var query = context.Wars
                .Where(w => w.IsTrainingWar == false && (w.CountryInWars.Any(ciw => ciw.CountryID == countryID)));

            query = applyActivityCriteria(criteria, query);

            return query;
        }
        public IQueryable<War> GetAllWarsForCountry(int countryID, WarActivitySearchCriteria criteria = WarActivitySearchCriteria.Active)
        {
            var query = context.Wars
                .Where(w => w.IsTrainingWar == false &&
                ((w.AttackerCountryID == countryID || w.DefenderCountryID == countryID)
                ||
                (w.CountryInWars.Any(ciw => ciw.CountryID == countryID))
                ));

            query = applyActivityCriteria(criteria, query);

            return query;
        }

        private static IQueryable<War> applyActivityCriteria(WarActivitySearchCriteria criteria, IQueryable<War> query)
        {
            if (criteria == WarActivitySearchCriteria.Active)
                query = query.Where(w => w.Active);
            else if (criteria == WarActivitySearchCriteria.Inactive)
                query = query.Where(w => w.Active == false);
            return query;
        }

        public IQueryable<Region> GetAttackableRegions(int warID, bool forAttacker)
        {
            

            Country country = null;

            if (forAttacker)
                country = Where(w => w.ID == warID)
                    .Select(w => w.Attacker)
                    .First();
            else
                country = Where(w => w.ID == warID)
                    .Select(w => w.Defender)
                    .First();



            Country otherCountry = null;

            if (!forAttacker)
                otherCountry = Where(w => w.ID == warID)
                    .Select(w => w.Attacker)
                    .First();
            else
                otherCountry = Where(w => w.ID == warID)
                    .Select(w => w.Defender)
                    .First();

            War war = GetById(warID);

            List<int> enemyCountriesID = null;

            if (forAttacker)
                enemyCountriesID = Where(w => w.ID == warID)
                    .SelectMany(w => w.CountryInWars)
                    .Where(ciw => ciw.IsAttacker == false)
                    .Select(ciw => ciw.CountryID)
                    .ToList();
            else
                enemyCountriesID = Where(w => w.ID == warID)
                    .SelectMany(w => w.CountryInWars)
                    .Where(ciw => ciw.IsAttacker == true)
                    .Select(ciw => ciw.CountryID)
                    .ToList();

            IQueryable<Region> neighbours = getCountryNeighbouringRegions(country);

            var test = neighbours.ToList();

            neighbours = neighbours.Where(n => n.CountryID.HasValue && (n.CountryID == otherCountry.ID || enemyCountriesID.Contains(n.CountryID.Value)));

            neighbours = removeActiveBattleRegions(neighbours);

            var activeBattlesRegionsIDs = context.Battles.Where(b => b.Active).Select(b => b.RegionID);

            neighbours = neighbours.Where(n => activeBattlesRegionsIDs.Contains(n.ID) == false);

            return neighbours;

        }



        private IQueryable<Region> getCountryNeighbouringRegions(Country country)
        {

            var countryRegions = context.Regions
                            .Where(r => r.CountryID == country.ID);

            var neighbours = countryRegions
                .SelectMany(r => r.Passages.Select(p => p.FirstRegion));

            var t1 = neighbours.ToList();

            neighbours = neighbours.Union(countryRegions
                .SelectMany(r => r.Passages.Select(p => p.SecondRegion)));

            var t2 = neighbours.ToList();

            neighbours = neighbours.Union(countryRegions
                .SelectMany(r => r.Passages1.Select(p => p.FirstRegion)));

            var t3 = neighbours.ToList();

            neighbours = neighbours.Union(countryRegions
                .SelectMany(r => r.Passages1.Select(p => p.SecondRegion)));

            var t4 = neighbours.ToList();

            neighbours = neighbours.Distinct();

            var t5 = neighbours.ToList();

            neighbours = neighbours.Where(n => n.CountryID != country.ID);

            var t6 = neighbours.ToList();

            neighbours = neighbours.Where(n => n.CountryID.HasValue);
            return neighbours;
        }

        private IQueryable<Region> removeActiveBattleRegions(IQueryable<Region> neighbours)
        {
            var activeBattleRegions = context.Battles.Where(b => b.Active).Select(b => b.ID);

            neighbours = neighbours.Where(n => activeBattleRegions.Contains(n.ID) == false);
            return neighbours;
        }

        public War GetWarAssociatedWithBattle(int battleID)
        {
            return FirstOrDefault(w => w.Battles.Any(b => b.ID == battleID));
        }
    }
}
