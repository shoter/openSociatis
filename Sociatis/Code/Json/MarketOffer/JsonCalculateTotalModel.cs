using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.structs;

namespace Sociatis.Code.Json.MarketOffer
{
    public class JsonCalculateTotalModel : JsonSuccessModel
    {
        public string ProductPrice { get; set; }
        public string FuelPrice { get; set; }

        public JsonCalculateTotalModel(OfferCost cost)
        {
            var currency = Persistent.Currencies.GetById(cost.CurrencyID);
            ProductPrice = $"{cost.CustomerCost} {currency.Symbol}";
            FuelPrice = $"{cost.IntegerRealCost} Fuel";
        }
    }
}