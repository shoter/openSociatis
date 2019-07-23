using Common.EntityFramework;
using Common.EntityFramework.SingleChanges;
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
    public class RegionRepository : RepositoryBase<Region, SociatisEntities>, IRegionRepository
    {
        public RegionRepository(SociatisEntities context) : base(context)
        {
        }

        public void AddPassage(Passage passage)
        {
            context.Passages.Add(passage);
        }

        public List<Passage> GetPassagesForRegion(int regionID)
        {
            return context.Passages
                .Where(p => p.FirstRegionID == regionID || p.SecondRegionID == regionID)
                .ToList();
        }

        /// <returns>Can return null if passage not found</returns>
        public Passage GetPassage(int firstRegionID, int secondRegionID)
        {
            return context.Passages
                .FirstOrDefault(
                p => (p.FirstRegionID == firstRegionID && p.SecondRegionID == secondRegionID)
                || (p.FirstRegionID == secondRegionID && p.SecondRegionID == firstRegionID));
        }

        /// <summary>
        /// Returns null if no resource
        /// </summary>
        public Resource GetResourceForRegion(int regionID, ResourceTypeEnum resourceType)
        {
            return context.Resources
                .FirstOrDefault(r => r.RegionID == regionID && r.ResourceTypeID == (int)resourceType);
        }

        public Region GetRegion(string regionName)
        {
            return FirstOrDefault(r => r.Name == regionName);
        }

        public Region GetRegion(string regionName, string countryName)
        {
            return First(r => r.Name == regionName && r.Country.Entity.Name == countryName);
        }

        public IQueryable<Region> GetCountryRegions(int countryID)
        {
            return Where(r => r.CountryID == countryID);
        }

        public List<RegionSpawnInformation> GetRegionsSpawnInformation(int countryID)
        {
            return Where(r => r.CountryID == countryID)
                .Select(r => new RegionSpawnInformation()
                {
                    RegionID = r.ID,
                    RegionName = r.Name,
                    SpawnEnabled = r.CanSpawn
                }).ToList();
        }

        public void ChangeSpawnSettings(Dictionary<int, bool> spawnSettings)
        {
            UpdateMany(spawnSettings, (set, region) => region.ID = set.Key,
              set => new SingleChangeExpression<Region, bool>(r => r.CanSpawn, set.Value));
        }

        public IQueryable<Region> GetSpawnableRegionsForCountry(int countryID)
        {
            return Where(r => r.CountryID == countryID && r.CanSpawn == true);
        }

        public IQueryable<Region> GetCoreRegionsForCountry(int countryID)
        {
            return Where(r => r.CountryCoreID == countryID);
        }
    }
}
