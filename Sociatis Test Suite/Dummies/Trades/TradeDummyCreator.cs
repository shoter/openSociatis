using Common;
using Common.Extensions;
using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis_Test_Suite.Dummies.Trades
{
    public class TradeDummyCreator : IDummyCreator<Trade>
    {
        private static UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private Trade trade;
        private static Random random = new Random();
        public TradeDummyCreator()
        {
            create();
        }

        private void create()
        {
            trade = new Trade()
            {
                DateTime = DateTime.Now,
                Day = GameHelper.CurrentDay,
                DestinationAccepted = false,
                SourceAccepted = false,
                TradeMoneys = new List<TradeMoney>(),
                TradeProducts = new List<TradeProduct>(),
                TradeStatusID = (int)TradeStatusEnum.Ongoing,
                ID = uniqueID,
                UpdatedDate = DateTime.Now
            };
            AddSourceEntity(new EntityDummyCreator().Create());
            AddDestinationEntity(new EntityDummyCreator().Create());
        }

        public TradeDummyCreator AcceptTrade(bool isSource)
        {
            if (isSource)
                trade.SourceAccepted = true;
            else
                trade.DestinationAccepted = true;
            return this;
        }

        public TradeDummyCreator SetUpdatedDate(DateTime time)
        {
            trade.UpdatedDate = time;
            return this;
        }

        public TradeDummyCreator SetStatus(TradeStatusEnum status)
        {
            trade.TradeStatusID = (int)status;
            return this;
        }

        public TradeDummyCreator AddRandomProduct(int quality, int amount, bool isSource)
        {
            return AddProduct(Enums.GetRandomValue<ProductTypeEnum>(), quality, amount, isSource);
        }

        public TradeDummyCreator AddRandomProduct()
        {
            return AddRandomProduct(1, random.Next(1, 10), random.NextBoolean());
        }

        public TradeDummyCreator AddProduct(ProductTypeEnum productType, int quality, int amount, bool isSource)
        {
            var tp = new TradeProduct()
            {
                Amount = amount,
                Quality = quality,
                ProductID = (int)productType,
                Entity = isSource ? trade.Source : trade.Destination,
                EntityID = isSource ? trade.SourceEntityID.Value : trade.DestinationEntityID.Value,
                Trade = trade,
                TradeID = trade.ID
            };

            trade.TradeProducts.Add(tp);
            return this;
        }

        public TradeDummyCreator AddRandomMoney()
        {
            return AddRandomMoney(random.Next(1, 100), random.NextBoolean());
        }

        public TradeDummyCreator AddRandomMoney(decimal amount, bool isSource)
        {
            var currency = new CurrencyDummyCreator().Create();
            return AddMoney(currency, amount, isSource);

        }

        public TradeDummyCreator AddMoney(Currency currency, decimal amount, bool isSource)
        {
            var tm = new TradeMoney()
            {
                Currency = currency,
                CurrencyID = currency.ID,
                Amount = amount,
                Entity = isSource ? trade.Source : trade.Destination,
                EntityID = isSource ? trade.SourceEntityID.Value : trade.DestinationEntityID.Value,
                Trade = trade,
                TradeID = trade.ID
            };

            trade.TradeMoneys.Add(tm);
            return this;
        }

        public TradeDummyCreator AddSourceEntity(Entity entity)
        {
            trade.Source = entity;
            trade.SourceEntityID = entity.EntityID;
            entity.SourceTrades.Add(trade);
            return this;
        }

        public TradeDummyCreator AddDestinationEntity(Entity entity)
        {
            trade.Destination = entity;
            trade.DestinationEntityID = entity.EntityID;
            entity.DestinationTrades.Add(trade);
            return this;
        }

        public Trade Create()
        {
            Trade temp = trade;
            create();
            return temp;
        }

        public Trade CreateRandom(int moneyCount, int productCount)
        {
            for (int i = 0; i < productCount; ++i)
                AddRandomProduct();
            for (int i = 0; i < moneyCount; ++i)
                AddRandomMoney();

            return Create();
        }
    }
}
