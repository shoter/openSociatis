using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class EventRepository : RepositoryBase<Event, SociatisEntities>, IEventRepository
    {
        public EventRepository(SociatisEntities context) : base(context)
        {
        }
    }

}
