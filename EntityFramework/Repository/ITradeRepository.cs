using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ITradeRepository : IRepository<Trade>
    {
        IQueryable<Trade> GetTradesThatAreHoursOld(int hourCount);
        IQueryable<Trade> GetTradesAssociatedWithEntity(int entityID);
    }
}
