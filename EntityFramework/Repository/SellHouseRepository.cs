using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class SellHouseRepository : RepositoryBase<SellHouse, SociatisEntities>, ISellHouseRepository
    {
        public SellHouseRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
