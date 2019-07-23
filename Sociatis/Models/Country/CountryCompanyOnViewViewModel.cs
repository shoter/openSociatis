using Common.Extensions;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Country
{
    public class CountryCompanyOnViewViewModel : SmallEntityAvatarViewModel
    {
        public int WorkersCount { get; set; }
        public string ShortDescription { get; set; }
        public int ConstructionDay { get; set; }
        public int Quality { get; set; }

        public CountryCompanyOnViewViewModel(Company company) : base(company.Entity)
        {
            WorkersCount = company.CompanyEmployees.Count;
            ConstructionDay = company.Entity.CreationDay;
            Quality = company.Quality;

            var productType = company.GetProducedProductType();
            switch (productType)
            {
                case ProductTypeEnum.SellingPower:
                    {
                        ShortDescription = "Company is a shop";
                        break;
                    }
                default:
                    {
                        TagBuilder producedProduct = new TagBuilder("span");
                        producedProduct.AddCssClass("producedProduct");
                        producedProduct.SetInnerText(productType.ToHumanReadable().FirstUpper());
                        ShortDescription = $"Company is producing {producedProduct.ToString()}.";
                        break;
                    }
            }
        }

    }
}