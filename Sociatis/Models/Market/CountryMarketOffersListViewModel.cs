using Common.utilities;
using Entities.enums;
using Entities.Models.Market;
using Sociatis.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils;

namespace Sociatis.Models.Market
{
    public class CountryMarketOffersListViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public List<MarketOfferViewModel> Offers { get; set; } = new List<MarketOfferViewModel>();
        public PagingParam PagingParam { get; set; }

        public int CountryID { get; set; }
        public int Quality { get; set; }
        public int ProductID { get; set; }

        public List<SelectListItem> ProductList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> QualityList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CountryList { get; set; } = new List<SelectListItem>();


        public CountryMarketOffersListViewModel(Entities.Entity entity, Entities.Country currentCountry, IList<MarketOfferModel> offers, IList<Entities.Country> countries, IList<int> allowedProductTypes, PagingParam pagingParam, int quality, int productID)
        {
            Info = new CountryInfoViewModel(currentCountry);
            var marketService = DependencyResolver.Current.GetService<IMarketService>();

            foreach (var offer in offers)
                Offers.Add(new MarketOfferViewModel(entity, offer, marketService));

            initSelf(currentCountry, countries, allowedProductTypes, pagingParam, quality, productID);

        }

        private void initSelf(Entities.Country currentCountry, IList<Entities.Country> countries, IList<int> allowedProductTypes, PagingParam pagingParam, int quality, int productID)
        {
            PagingParam = pagingParam;
            CountryID = currentCountry.ID;
            ProductID = productID;
            Quality = quality;

            SelectListItem emptyItem = new SelectListItem()
            {
                Text = "-- Select --",
                Value = "0"
            };

            initProducts(allowedProductTypes, emptyItem);

            initQuality(emptyItem);

            initCountries(currentCountry, countries);
        }

        public void DisableShowingQuantity()
        {
            Offers.ForEach(x => x.ShowQuantity = false);
        }

        private void initQuality(SelectListItem emptyItem)
        {
            for (int i = 1; i <= 5; ++i)
                QualityList.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            QualityList.Add(emptyItem);
        }

        private void initProducts(IList<int> allowedProductTypes, SelectListItem emptyItem)
        {
            foreach (ProductTypeEnum product in allowedProductTypes)
            {
                ProductList.Add(new SelectListItem()
                {
                    Value = product.ToInt().ToString(),
                    Text = product.ToHumanReadable().FirstToUpper()
                });
            }
            ProductList.Add(emptyItem);

            ProductList = ProductList.OrderBy(p => p.Text).ToList();
        }

        private void initCountries(Entities.Country currentCountry, IList<Entities.Country> countries)
        {
            foreach (var country in countries)
            {
                CountryList.Add(new SelectListItem()
                {
                    Text = country.Entity.Name,
                    Value = country.ID.ToString(),
                    Selected = country.ID == currentCountry.ID
                });
            }
        }
    }
}