using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IHouseRepository : IRepository<House>
    {
        bool HasHouseInRegion(int citizenID, int regionID);
        IEnumerable<House> GetOwnedHouses(int citizenID);
    }
}
