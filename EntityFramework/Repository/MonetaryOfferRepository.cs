using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using Entities.structs.MonetaryMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class MonetaryOfferRepository : RepositoryBase<MonetaryOffer, SociatisEntities>, IMonetaryOfferRepository
    {
        public MonetaryOfferRepository(SociatisEntities context) : base(context)
        {
        }
        public ActualBuySellOffers GetActualBuySellOffer(int sellCurencyID, int buyCurrencyID)
        {
            var _ret = new ActualBuySellOffers();
            var offers = Where(o => o.SellCurrencyID == sellCurencyID && o.BuyCurrencyID == buyCurrencyID);

            var bestBuyOffer = offers.Where(o => o.OfferTypeID == (int)MonetaryOfferTypeEnum.Buy)
                .GroupBy(o => o.Rate)
                .OrderByDescending(o => o.Key)
                .Select(g => new
                {
                    Rate = g.Key,
                    Volume = g.Sum(o => o.Amount)
                }).FirstOrDefault();

            if(bestBuyOffer != null)
            {
                _ret.BuyPrice = (double)bestBuyOffer.Rate;
                _ret.BuyVolume = bestBuyOffer.Volume;
            }

            var bestSellOffer = offers.Where(o => o.OfferTypeID == (int)MonetaryOfferTypeEnum.Sell)
                .GroupBy(o => o.Rate)
                .OrderBy(o => o.Key)
                .Select(g => new
                {
                    Rate = g.Key,
                    Volume = g.Sum(o => o.Amount)
                }).FirstOrDefault();

            if (bestSellOffer != null)
            {
                _ret.SellPrice = (double)bestSellOffer.Rate;
                _ret.SellVolume = bestSellOffer.Volume;
            }

            return _ret;

        }

        public Dictionary<int, ReservedMoney> GetReservedMoney(int entityID)
        {
            return Where(o => o.SellerID == entityID)
                .Select(o => new
                {
                    OfferReservedMoney = o.OfferReservedMoney,
                    TaxReservedMoney = o.TaxReservedMoney,
                    CurrencyID = o.OfferTypeID == (int)MonetaryOfferTypeEnum.Buy ? o.SellCurrencyID : o.BuyCurrencyID
                }).GroupBy(o => o.CurrencyID)
                .Select(grp => new
                {
                    OfferReservedMoney = (double)grp.Sum(o => o.OfferReservedMoney),
                    TaxReservedMoney = (double)grp.Sum(o => o.TaxReservedMoney),
                    CurrencyID = grp.Key
                })
                .ToDictionary(o => o.CurrencyID, o => new ReservedMoney() {
                    OfferReserved = o.OfferReservedMoney,
                    TaxReserved = o.TaxReservedMoney
                });
        }
    }
}
