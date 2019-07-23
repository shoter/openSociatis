using Common.Extensions;
using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Trades
{
    public class ProductForTradeViewModel : ItemForTradeViewModel
    {
        public int ProductID { get; set; }

        public ImageViewModel Image { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Quality { get; set; }

        public ProductForTradeViewModel(TradeProduct product, TradeStatusEnum tradeStatus) : base(product.EntityID, product.DateAdded, product.TradeID, tradeStatus)
        {
            ProductID = product.ProductID;

            var productType = (ProductTypeEnum)product.ProductID;

            Image = Images.GetProductImage(productType).VM;
            Name = productType.ToHumanReadable().FirstUpper();
            Quality = product.Quality;

            Amount = product.Amount;
        }
    }
}