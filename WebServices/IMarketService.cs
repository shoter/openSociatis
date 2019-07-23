using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams.MarketOffers;
using WebServices.structs;

namespace WebServices
{
    public interface IMarketService
    {
        MarketOffer AddOffer(AddMarketOfferParameters ps);
        MethodResult RemoveOffer(int offerID);
        MethodResult Buy(int offerID, int buyerID, int amount);
        MethodResult Buy(MarketOffer offer, Entity buyer, int amount);
        MethodResult Buy(MarketOffer offer, Entity buyer, int amount, int walletID);
        MethodResult CanBuy(MarketOffer offer, Entity buyer, Entity seller);
        MethodResult CanBuy(int offerID, Entity buyer, Entity seller);
        OfferCost GetOfferCost(MarketOffer offer, Entity buyer, int amount);
        decimal? GetFuelCostForOffer(MarketOffer offer, Entity buyer);
        ProductCost CalculateProductCost(int amount, decimal nettoPrice, int? homeCountryID, int? sellingCountryID, ProductTypeEnum productType);
        decimal GetVatForProduct(int? countryID, int productID);

        MethodResult CanMakeOffer(Company company, int amount, decimal price, ProductTypeEnum productType, int quality, int? countryID, List<int> embargoedCountries);

        OfferCost GetOfferCost(MarketOfferModel offer, Entity buyer, int amount);
        decimal? GetFuelCostForOffer(MarketOfferModel offer, Entity buyer);
        bool CanUseFuel(Entity entity);

        MethodResult CanBuyOffer(MarketOffer offer, int amount, Entity entity);

        bool IsEnoughFuelForTrade(int? fuelInInventory, int neededFuel, MarketOffer offer);

        decimal? GetFuelCostForOffer(int marketOfferID, Region destinationRegion);
        OfferCost GetOfferCost(MarketOffer offer, int amount, Region destinationRegion, bool useFuel);
        MethodResult CanBuyOffer(MarketOffer offer, int amount, Entity entity, int walletID);
    }
}
