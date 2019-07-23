using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class RegionPersistentRepository : PersistentRepositoryBase<Region>
    {
        private List<Passage> passages = new List<Passage>();

        public RegionPersistentRepository() : base(new PersistentListStorage<Region>())
        {
        }

        protected override void LoadEntity(Region persistentEntity, Region dbEntity)
        {
            Load(e => e.Passages, persistentEntity, dbEntity);
            Load(e => e.Passages1, persistentEntity, dbEntity);

            passages.AddRange(persistentEntity.Passages);
            passages.AddRange(persistentEntity.Passages1);

            
        }

        protected override void AfterLoad()
        {
            foreach (var passage in passages)
            {
                passage.FirstRegion = storage.First(r => r.ID == passage.FirstRegionID);
                passage.SecondRegion = storage.First(r => r.ID == passage.SecondRegionID);
            }
        }

        public override Region GetById(int id)
        {
            return storage.FirstOrDefault(region => region.ID == id);
        }
        public void BasicPreload()
        {
            var context = new SociatisEntities();

            var set = context.Set<Region>();

            var basicInformations =
                context.Regions
                .Select(r => new { RegionID = r.ID, CountryID = r.CountryID, Developement = r.Development })
                .ToList();

            foreach(var info in basicInformations)
            {
                var region = storage.First(r => r.ID == info.RegionID);
                region.CountryID = info.CountryID;
                region.Development = info.Developement;
            }
        }
    }
}
