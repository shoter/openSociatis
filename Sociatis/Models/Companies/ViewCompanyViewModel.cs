using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ViewCompanyViewModel
    {
        public CompanyInfoViewModel Info { get; set; }
        public int Quality { get; set; }
        public List<MoneyViewModel> Money { get; set; } = new List<MoneyViewModel>();

        public List<ProductRequirementViewModel> ProductionRequirements { get; set; } = new List<ProductRequirementViewModel>();
        public ProductStockViewModel Stock { get; set; }
        /// <summary>
        /// true if current entity is working here
        /// </summary>
        public bool CitizenWorkingSite { get; set; } = false;
        public bool CanWork { get; set; } = false;
        public double ProduceAmount { get; set; }
        public bool ShowEmployeeStats { get; set; }
        public string OwnerName { get; set; }
        public double Queue { get; set; }
        public int OwnerID { get; set; }

        public string CannotWorkReason { get; set; }
        public int DayWorkInRow { get; set; } = 0;


        public CompanyRights Rights { get; set; }

        public ViewCompanyEmployeesViewModel Employees { get; set; }

        public CompanyStatisticsViewModel Statistics { get; set; }


        public ViewCompanyViewModel(Company company, IProductRepository productRepository, ICompanyService companyService, CompanyRights companyRights, IRegionService regionService, IRegionRepository regionRepository)
        {
            IProductService productService = DependencyResolver.Current.GetService<IProductService>();

            Info = new CompanyInfoViewModel(company);
            Quality = company.Quality;
            Money = MoneyViewModel.GetMoney(company.Entity.Wallet);
            ShowEmployeeStats = true;
            Queue = (double)company.Queue;
            Rights = companyRights;

            initializeEmployees(company, companyService);

            initializeRequiredProducts(company, productRepository, productService);

            initializeProducedStock(company, productRepository);

            if (Rights.HaveAnyRights)
                Statistics = new CompanyStatisticsViewModel(company, companyService, regionService, regionRepository);
        }

        private void initializeProducedStock(Company company, IProductRepository productRepository)
        {
            ProductTypeEnum producedProduct = (ProductTypeEnum)company.ProductID;
            var producedProductEntity = company.Entity.GetEquipmentItem(producedProduct, company.Quality, productRepository);

            Stock = new ProductStockViewModel(company, producedProductEntity);
        }

        private void initializeRequiredProducts(Company company, IProductRepository productRepository, IProductService productService)
        {
            var requiredProducts = productService.GetRequiredRaws((ProductTypeEnum)company.ProductID, company.Quality);

            foreach (var requiredProduct in requiredProducts)
            {
                ProductTypeEnum productType = requiredProduct.RequiredProductType;

                var item = company.Entity.GetEquipmentItem(productType, requiredProduct.Quality, productRepository);

                var pVM = new ProductRequirementViewModel(requiredProduct, item.Amount);
                ProductionRequirements.Add(pVM);
            }
        }

        private void initializeEmployees(Company company, ICompanyService companyService)
        {
            List<CompanyEmployeeViewModel> employees = new List<CompanyEmployeeViewModel>();
            foreach (var employee in company.CompanyEmployees)
            {
                CompanyEmployeeViewModel cvm = new CompanyEmployeeViewModel(employee);

                if (SessionHelper.CurrentEntity.Citizen != null)
                {
                    var citizen = SessionHelper.CurrentEntity.Citizen;

                    if(cvm.ID == citizen.ID) //if it's our citizen that we are currently playing
                    {
                        CitizenWorkingSite = true;
                        ProduceAmount = Math.Round(cvm.Production ?? 0, 1);
                        var result = companyService.CanWork(citizen, company);

                        CanWork = result.IsError == false;

                        DayWorkInRow = citizen.DayWorkedRow;

                        if (result.IsError)
                            CannotWorkReason = result.Errors[0];


                    }
                }
                employees.Add(cvm);
            }

            Employees = new ViewCompanyEmployeesViewModel(employees, ShowEmployeeStats, Rights);
        }


    }
}
