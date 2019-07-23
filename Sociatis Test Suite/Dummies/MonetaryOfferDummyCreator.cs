using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class MonetaryOfferDummyCreator : IDummyCreator<MonetaryOffer>
    {
        private static UniqueIDGenerator ID = new UniqueIDGenerator();
        private MonetaryOffer offer;
        private int sellCurrencyID = 1;
        private int buyCurrencyID = 2;
        private MonetaryOfferTypeEnum monetaryOfferType;

        public MonetaryOfferDummyCreator()
        {
            offer = create();
        }

        public MonetaryOfferDummyCreator SetSellCurrency(int currencyID)
        {
            sellCurrencyID = currencyID;
            return this;
        }

        public MonetaryOfferDummyCreator SetBuyCurrency(int currencyID)
        {
            buyCurrencyID = currencyID;
            return this;
        }

        public MonetaryOfferDummyCreator SetMonetaryOfferType(MonetaryOfferTypeEnum monetaryOfferType)
        {
            this.monetaryOfferType = monetaryOfferType;
            reset();
            return this;
        }

        private void reset() { offer = create(); }

        private MonetaryOffer create()
        {
            offer = new MonetaryOffer()
            {
                ID = ID.UniqueID,
                SellCurrencyID = sellCurrencyID,
                BuyCurrencyID = buyCurrencyID,
                Amount = 1,
                Rate = 1,
                OfferTypeID = (int)monetaryOfferType,
                SellerID = -1 //propably will be not used
            };

            return offer;
        }

        public MonetaryOffer Create()
        {
            var temp = offer;
            offer = create();
            return temp;
        }

        public MonetaryOffer Create(double rate, int amount = 1)
        {
            var offer = Create();
            offer.Rate = (decimal)rate;
            offer.Amount = amount;
            return offer;
        }
    }
}
