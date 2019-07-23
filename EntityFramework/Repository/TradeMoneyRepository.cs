using Common.EntityFramework;
using Entities.structs.Trades;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class TradeMoneyRepository : RepositoryBase<TradeMoney, SociatisEntities>, ITradeMoneyRepository
    {
        public TradeMoneyRepository(SociatisEntities context) : base(context)
        {
        }

        public IQueryable<TradeMoney> FindMany<T>(ICollection<TradeMoneyFindArgs> args)
        {
            {
                var concatenatedIDs = args.Select(el => el.TradeID.ToString() + "," + el.CurrencyID.ToString() + "," + el.EntityID.ToString());

#if DEBUG
                var test = DbSet.Select(product =>
                    SqlFunctions.StringConvert((decimal)product.TradeID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.CurrencyID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.EntityID).Trim()
                    ).ToList(); ;

                var test2 = DbSet.ToList();
#endif


                return DbSet.Where(product => concatenatedIDs.Contains(
                    SqlFunctions.StringConvert((decimal)product.TradeID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.CurrencyID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.EntityID).Trim()
                    ));
            }
        }
    }
}
