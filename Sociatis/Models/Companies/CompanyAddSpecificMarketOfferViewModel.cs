using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using Entities.enums;
using Common.utilities;
using System.Web.Mvc;
using WebServices;
using Sociatis.Helpers;

namespace Sociatis.Models.Companies
{
    public class CompanyAddSpecificMarketOfferViewModel : CompanyAddMarketOfferViewModel
    {
        public string ProductName { get; set; }
        public ImageViewModel ProductImage { get; set; }

        public CompanyAddSpecificMarketOfferViewModel(Company company, List<int> embargoedCountries, ProductTypeEnum productType, int productQuality) : base(company, embargoedCountries)
        {
            ProductID = (int)productType;
            ProductName = productType.ToHumanReadable().FirstToUpper();
            Quality = productQuality;

            ProductImage = Images.GetProductImage(productType).VM;
        }

        public override void LoadSelectLists(List<int> embargoedCountries, Company company)
        {
            Countries.Add(new SelectListItem()
            {
                Text = "No public offer",
                Value = "",
                Selected = true
            });

            foreach (var country in Persistent.Countries.GetAll())
            {
                if (embargoedCountries.Contains(country.ID))
                    continue;

                var item = new SelectListItem()
                {
                    Value = country.ID.ToString(),
                    Text = country.Entity.Name
                };

                Countries.Add(item);
            }

        }
    }
}