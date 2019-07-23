using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Market;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IMarketOfferRepository : IRepository<MarketOffer>
    {
        IQueryable<MarketOfferModel> GetMarketOfferModel();
        IQueryable<MarketOfferModel> GetAvailableOffers(int productID, int quality, Entities.Country country, CompanyTypeEnum[] companyTypes, params int[] productTypes);
    }
}
