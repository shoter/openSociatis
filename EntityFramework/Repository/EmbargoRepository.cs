using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class EmbargoRepository : RepositoryBase<Embargo, SociatisEntities>, IEmbargoRepository
    {
        public EmbargoRepository(SociatisEntities context) : base(context)
        {
        }

        public List<Embargo> GetAllActiveEmbargoes()
        {
            return Where(e => e.Active)
                .ToList();
        }
    }
}
