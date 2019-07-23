using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Regions;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IRegionRepository : IRepository<Region>
    {
        void AddPassage(Passage passage);
        List<Passage> GetPassagesForRegion(int regionID);
        Passage GetPassage(int firstRegionID, int secondRegionID);

        /// <summary>
        /// Returns null if no resource
        /// </summary>
        Resource GetResourceForRegion(int regionID, ResourceTypeEnum resourceType);
        Region GetRegion(string regionName);
        Region GetRegion(string regionName, string countryName);

        List<RegionSpawnInformation> GetRegionsSpawnInformation(int countryID);

        void ChangeSpawnSettings(Dictionary<int, bool> spawnSettings);
        IQueryable<Region> GetSpawnableRegionsForCountry(int countryID);
        IQueryable<Region> GetCoreRegionsForCountry(int countryID);
        IQueryable<Region> GetCountryRegions(int countryID);
    }
}
