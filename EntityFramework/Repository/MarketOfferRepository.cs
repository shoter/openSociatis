using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Market;
using Entities.Repository.Base;
using Entities.structs.MonetaryMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class MarketOfferRepository : RepositoryBase<MarketOffer, SociatisEntities>, IMarketOfferRepository
    {
        public MarketOfferRepository(SociatisEntities context) : base(context)
        {
        }

        public IQueryable<MarketOfferModel> GetMarketOfferModel()
        {
            return
                (from offer in context.MarketOffers
                 join company in context.Companies on offer.CompanyID equals company.ID
                 join region in context.Regions on company.RegionID equals region.ID
                 join dbCountry in context.Entities on region.CountryID equals dbCountry.EntityID
                 join entity in context.Entities on company.ID equals entity.EntityID
                 join equipmentItem in context.EquipmentItems.Where(ei => ei.ProductID == (int)ProductTypeEnum.SellingPower) on entity.EquipmentID equals equipmentItem.EquipmentID into equipmentItems
                 select new MarketOfferModel()

                 {
                     //Amount = offer.Amount,
                     Amount = (equipmentItems.FirstOrDefault().Amount > offer.Amount || company.CompanyTypeID != (int)CompanyTypeEnum.Shop) ? offer.Amount : (int?)equipmentItems.FirstOrDefault().Amount ?? 0,
                     Company = entity,
                     CompanyCountryName = dbCountry.Name,
                     CompanyCountryID = dbCountry.EntityID,
                     CompanyRegionID = region.ID,
                     CountryID = offer.CountryID,
                     CurrencyID = offer.CurrencyID,
                     OfferID = offer.ID,
                     Price = offer.Price,
                     ProductID = offer.ProductID,
                     Quality = offer.Quality,
                     CompanyTypeID = company.CompanyTypeID
                 });
        }

        public IQueryable<MarketOfferModel> GetAvailableOffers(int productID, int quality, Entities.Country country, CompanyTypeEnum[] companyTypes, params int[] productTypes)
        {

            var query = GetMarketOfferModel()
                 .Where(o => o.Amount > 0 && o.CountryID == country.ID && productTypes.Contains(o.ProductID));

            if (productID != 0)
                query = query.Where(o => o.ProductID == productID);
            if (quality != 0)
                query = query.Where(o => o.Quality == quality);

            var intCompanyTypes = companyTypes.Select(c => c.ToInt());


            query = query.Where(o => intCompanyTypes.Contains(o.CompanyTypeID));

            query = query.OrderBy(o => o.Price);

            return query;
        }
    }
}
