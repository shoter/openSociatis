using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class TradeRepository : RepositoryBase<Trade, SociatisEntities>, ITradeRepository
    {
        public TradeRepository(SociatisEntities context) : base(context)
        {
            
        }

        public IQueryable<Trade> GetTradesThatAreHoursOld(int hourCount)
        {
            DateTime timeBoundary = DateTime.Now.AddHours(-hourCount);
            return Where(trade => trade.UpdatedDate <= timeBoundary);
        }

        public IQueryable<Trade> GetTradesAssociatedWithEntity(int entityID)
        {
            return Where(trade => trade.SourceEntityID == entityID || trade.DestinationEntityID == entityID);
        }


    }
}
