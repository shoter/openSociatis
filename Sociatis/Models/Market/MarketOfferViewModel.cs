using Common.Extensions;
using Entities;
using Entities.enums;
using Entities.Models.Market;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Market
{
    public class MarketOfferViewModel
    {
        public int OfferID { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public int Quality { get; set; }
        public int CurrencyID { get; set; }
        public decimal? Price { get; set; }
        public string PriceSymbol { get; set; }
        public decimal? FuelPrice { get; set; } = null;
        public decimal? FuelPricePer { get; set; } 
        public bool IsPostedOnMarket { get; set; }
        public string MarketCountryName { get; set; }
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public int Tax { get; set; }
        public ImageViewModel ProductImage { get; set; }
        public ImageViewModel ProductAvatar { get; set; }
        public bool Deleteable { get; set; } = false;
        public bool ShowDetails { get; set; } = false;

        public bool CanBuy { get; set; }
        public bool ShowQuantity { get; set; } = true;

        public MarketOfferViewModel(Entity entity, MarketOfferModel offer, IMarketService marketService, bool deleteable = false, bool showDetails = false)
        {
            var cost = marketService.GetOfferCost(offer, entity, 1);

            initEntityless(offer, deleteable, showDetails, cost);

            initSelf(offer, entity, marketService, cost);

        }

        private void initSelf(MarketOfferModel offer, Entity entity, IMarketService marketService, WebServices.structs.OfferCost cost)
        {
            CanBuy = marketService.CanBuy(offer.OfferID, entity, offer.Company).isSuccess;

            if ((EntityTypeEnum)entity.EntityTypeID != EntityTypeEnum.Citizen && (EntityTypeEnum)entity.EntityTypeID != EntityTypeEnum.Newspaper)
                FuelPrice = cost.FuelCost;

            if (marketService.CanUseFuel(entity))
            {
                FuelPricePer = marketService.GetFuelCostForOffer(offer, entity);

                if (FuelPricePer.HasValue)
                    FuelPricePer = Math.Round(FuelPricePer.Value, 2);
            }
        }

        private void initEntityless(MarketOfferModel offer, bool deleteable, bool showDetails, WebServices.structs.OfferCost cost)
        {
            ProductTypeEnum productType = (ProductTypeEnum)offer.ProductID;

            CurrencyID = offer.CurrencyID;
            OfferID = offer.OfferID;
            ProductName = productType.ToHumanReadable().FirstUpper();
            Quality = offer.Quality;

            Price = cost.BasePrice + cost.VatCost;
            Amount = offer.Amount;
            IsPostedOnMarket = offer.CountryID > 0;

            if (IsPostedOnMarket)
            {
                MarketCountryName = offer.CompanyCountryName;
            }

            CompanyID = offer.Company.EntityID;
            CompanyName = offer.Company.Name;
            ProductImage = Images.GetProductImage(productType).VM;
            ProductAvatar = getProductAvatar(offer);

            var currency = Persistent.Currencies.GetById(offer.CurrencyID);
            PriceSymbol = currency.Symbol;

            Deleteable = deleteable;
            ShowDetails = showDetails;

            IProductService productService = DependencyResolver.Current.GetService<IProductService>();

            Tax = (int)(productService.GetAllTaxesForProduct(offer.ProductID, offer.CompanyCountryID, null).VAT * 100m);
        }

        private ImageViewModel getProductAvatar(MarketOfferModel offer)
        {
            if (offer.Company.ImgUrl == null)
                return new ImageViewModel(Images.GetProductImageHighRes((ProductTypeEnum)offer.ProductID));

             return new ImageViewModel(offer.Company.ImgUrl);
        }
    }
}