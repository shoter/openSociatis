using Common.utilities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Companies
{
    public class CompanyAddMarketOfferViewModel
    {
        public CompanyInfoViewModel Info { get; set; }

        public int ProductID { get; set; }
        [Required]
        public int? Amount { get; set; }
        public int Quality { get; set; }
        public int? CountryID { get; set; }
        [Required]
        public decimal? Price { get; set; }

        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Countries { get; set; } = new List<SelectListItem>();
        public Dictionary<string, List<int>> ProductQualities { get; set; } = new Dictionary<string, List<int>>();

        public CompanyAddMarketOfferViewModel() { }

        public CompanyAddMarketOfferViewModel(Entities.Company company, List<int> embargoedCountries)
        {
            Info = new CompanyInfoViewModel(company);

            LoadSelectLists(embargoedCountries, company);
        }

        public virtual void LoadSelectLists(List<int> embargoedCountries, Entities.Company company)
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


            foreach (var item in company.Entity
                .Equipment
                .EquipmentItems
                .Select(e => e.ProductID)
                .Distinct()
                .ToList())
            {
                var product = (ProductTypeEnum)item;

                if (CompanyService.IsSellable(product) == false)
                    continue;

                if (CompanyService.CanSellProduct((CompanyTypeEnum)company.CompanyTypeID, product) == false)
                    continue;

                Products.Add(new SelectListItem()
                {
                    Value = ((int)product).ToString(),
                    Text = product.ToHumanReadable().FirstToUpper()
                });

   
            }

            foreach (var item in company.Entity
                .Equipment
                .EquipmentItems
                .Select(e => new { ProductID = e.ProductID, Quality = e.Quality })
                .Distinct()
                .ToList())
            {
                if (CompanyService.IsSellable((ProductTypeEnum)item.ProductID) == false)
                    continue;


                if(ProductQualities.ContainsKey(item.ProductID.ToString()) == false)
                {
                    ProductQualities.Add(item.ProductID.ToString(), new List<int>());
                }

                ProductQualities[item.ProductID.ToString()].Add(item.Quality);
            }


        }
    }
}