using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HotelManagerRepository : RepositoryBase<HotelManager, SociatisEntities>, IHotelManagerRepository
    {
        public HotelManagerRepository(SociatisEntities context) : base(context)
        {
        }

        public HotelManager Get(int hotelID, int managerID)
        {
            return FirstOrDefault(m => m.HotelID == hotelID && m.CitizenID == managerID);
        }
    }
}
