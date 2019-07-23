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
    public class TradeProductRepository : RepositoryBase<TradeProduct, SociatisEntities>, ITradeProductRepository
    {
        public TradeProductRepository(SociatisEntities context) : base(context)
        {
        }

        public IQueryable<TradeProduct> FindMany<T>(ICollection<TradeProductFindArgs> args)
        {
            {
                var concatenatedIDs = args.Select(el => el.TradeID.ToString() + "," + el.ProductID.ToString() + "," + el.EntityID.ToString() + "," + el.Quality.ToString());

#if DEBUG
                var test = DbSet.Select(product =>
                    SqlFunctions.StringConvert((decimal)product.TradeID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.ProductID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.EntityID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.Quality).Trim()
                    ).ToList(); ;

                var test2 = DbSet.ToList();
#endif


                return DbSet.Where(product => concatenatedIDs.Contains(
                    SqlFunctions.StringConvert((decimal)product.TradeID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.ProductID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.EntityID).Trim() +
                    "," +
                    SqlFunctions.StringConvert((decimal)product.Quality).Trim()
                    ));
            }
        }
    }
}
