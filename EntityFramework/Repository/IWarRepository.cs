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
    public interface IWarRepository : IRepository<War>
    {
        IQueryable<CountryInWar> GetCountriesInWar(int warID);
        IQueryable<War> GetDirectWarsForCountry(int countryID, WarActivitySearchCriteria criteria = WarActivitySearchCriteria.Active);
        IQueryable<War> GetIndirectWarsForCountry(int countryID, WarActivitySearchCriteria criteria = WarActivitySearchCriteria.Active);
        IQueryable<War> GetAllWarsForCountry(int countryID, WarActivitySearchCriteria criteria = WarActivitySearchCriteria.Active);
        IQueryable<Region> GetAttackableRegions(int warID, bool forAttacker);
        IQueryable<Battle> GetActiveBattles(int countryID);
        List<War> GetAllActiveWars();

        Battle GetTrainingBattle();

        War GetWarAssociatedWithBattle(int battleID);
    }
}
