using Common.Extensions;
using Entities.enums;
using Entities.Repository;
using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Companies
{
    public class CreateCompanyViewModel : ViewModelBase
    {
        [Required]
        [DisplayName("Company Name")]
        [StringLength(30, ErrorMessage = "Must be between 2 and 30 characters", MinimumLength = 2)]
        public string CompanyName { get; set; }
        [DisplayName("Region")]
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        [DisplayName("Country")]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        [DisplayName("Fee")]
        public MoneyViewModel CountryFee { get; set; }
        [DisplayName("Fee")]
        public MoneyViewModel AdminFee { get; set; }
        
        [Required]
        [DisplayName("Produced product")]
        public int? ProducedProductID { get; set; }
        public List<SelectListItem> ProducedProductTypes { get; set; } = new List<SelectListItem>();

        public CreateCompanyViewModel()
        {
            ProducedProductTypes = new List<SelectListItem>();
        }

        public void LoadSelectList(ICompanyRepository companyRepository, ICompanyService companyService)
        {
            var products = companyRepository.GetProducts();

            var allowedProducts = companyService.GetCompaniesProductsCreatableByPlayers();

            foreach (var product in products)
            {
                var productType = (ProductTypeEnum)product.ID;
                if (allowedProducts.Contains(productType) == false)
                    continue;

                string name = productType.ToHumanReadable().FirstUpper();

                switch (productType)
                {
                    case ProductTypeEnum.MedicalSupplies:
                        name = "Hospital"; break;
                    case ProductTypeEnum.SellingPower:
                        name = "Shop"; break;
                }

                if (productType.GetCompanyTypeForProduct() == CompanyTypeEnum.Construction)
                    name = $"Construction company - {name}";




                ProducedProductTypes.Add(new SelectListItem()
                {
                    Text = name,
                    Value = product.ID.ToString()
                });

            }
        }
    }
}