using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class PolygonRepository : RepositoryBase<Polygon, SociatisEntities>, IPolygonRepository
    {
        public PolygonRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
