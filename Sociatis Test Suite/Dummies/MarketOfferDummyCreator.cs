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
    public class MarketOfferDummyCreator : IDummyCreator<MarketOffer>
    {
        private UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private MarketOffer offer;
        private CompanyDummyCreator companyDummyCreator = new CompanyDummyCreator();

        public MarketOfferDummyCreator()
        {
            offer = create();
        }

        private MarketOffer create()
        {
            var company = companyDummyCreator.Create();
            return new MarketOffer()
            {
                ID = uniqueID,
                Amount = 1,
                Quality = 1,
                ProductID = (int)ProductTypeEnum.Bread,
                Company = company,
                CompanyID = company.ID,
                Currency = company.Region.Country.Currency,
                CurrencyID = company.Region.Country.CurrencyID,
                Price = 1.00m,
                
            };
        }

        public MarketOfferDummyCreator SetCompany(Company company)
        {
            offer.Company = company;
            offer.CompanyID = company.ID;

            return this;
        }

        public MarketOfferDummyCreator SetCountry(Country country)
        {
            offer.Country = country;
            offer.CountryID = country?.ID;

            country.MarketOffers.Add(offer);

            return this;
        }

        public MarketOfferDummyCreator SetRegion(Region region)
        {
            offer.Company.Region = region;
            offer.Company.RegionID = region.ID;

            return this;
        }

        public MarketOfferDummyCreator SetAmount(int amount)
        {
            offer.Amount = amount;
            return this;
        }
        public MarketOfferDummyCreator SetQuality(int quality)
        {
            offer.Quality = quality;
            return this;
        }
        public MarketOfferDummyCreator SetProduct(ProductTypeEnum productType)
        {
            offer.ProductID = (int)productType;
            return this;
        }
        public MarketOffer Create()
        {
            var tmp = offer;
            offer = create();
            return tmp;
        }
    }
}
