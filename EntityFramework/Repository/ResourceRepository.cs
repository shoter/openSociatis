using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ResourceRepository : RepositoryBase<Resource, SociatisEntities>, IResourceRepository
    {
        public ResourceRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
