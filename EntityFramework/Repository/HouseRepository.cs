using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HouseRepository : RepositoryBase<House, SociatisEntities>, IHouseRepository
    {
        public HouseRepository(SociatisEntities context) : base(context)
        {
        }

        public bool HasHouseInRegion(int citizenID, int regionID)
        {
            return
                 Any(h => h.RegionID == regionID && h.CitizenID == citizenID);
        }

        public IEnumerable<House> GetOwnedHouses(int citizenID)
        {
            return Where(h => h.CitizenID == citizenID)
                .ToList();
        }
    }
}
