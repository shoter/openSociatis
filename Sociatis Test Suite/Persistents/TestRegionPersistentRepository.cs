using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Sociatis_Test_Suite.Persistents
{
    public class TestRegionPersistentRepository : RegionPersistentRepository
    {
        public TestRegionPersistentRepository()
        {
        }

        public override void Add(Region t)
        {
            storage.Add(t);
        }
    }
}
