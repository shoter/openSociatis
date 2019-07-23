using Entities.Models.Market;
using Sociatis.Helpers;
using Sociatis.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.structs;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Models.Companies
{
    public class CompanyMarketOfferListViewModel
    {
        public CompanyInfoViewModel Info { get; set; }
        public CompanyRights CompanyRights { get; set; }
        public List<MarketOfferViewModel> Offers { get; set; } = new List<MarketOfferViewModel>();
        public PagingParam PagingParam { get; set; }

        public CompanyMarketOfferListViewModel(Entities.Company company, IQueryable<MarketOfferModel> marketOffers, PagingParam pagingParam,
            CompanyRights rights, IMarketService marketService)
        {
            Info = new CompanyInfoViewModel(company);
            CompanyRights = rights;

            PagingParam = pagingParam;
            if (marketOffers.Count() > 0)
            {
                var filteredOffers = marketOffers.OrderByDescending(mo => mo.OfferID).Apply(PagingParam).ToList();

                foreach (var offer in filteredOffers)
                    Offers.Add(new MarketOfferViewModel(SessionHelper.CurrentEntity, offer,  marketService, deleteable: true, showDetails: true));
            }
            
        }
    }
}