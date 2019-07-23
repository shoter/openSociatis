using Common.EntityFramework;
using Entities.Repository.Base;
using Entities.structs.MonetaryMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IMonetaryOfferRepository : IRepository<MonetaryOffer>
    {
        ActualBuySellOffers GetActualBuySellOffer(int sellCurencyID, int buyCurrencyID);
        /// <summary>
        /// Returns sum of currency used in monetary offers. Data is grouped by currency ID.
        /// </summary>
        /// <param name="entityID">ID of entity for which we will seek for reserved money</param>
        /// <returns>dictionary. Key is currency ID, double is amount of currency reserved.</returns>
        Dictionary<int, ReservedMoney> GetReservedMoney(int entityID);
    }
}
