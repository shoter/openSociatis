using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using Entities.Repository;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ViewDevelopementCompanyViewModel : ViewCompanyViewModel
    {
        public string Infrastructure { get; set; }
        public ViewDevelopementCompanyViewModel(Company company, IProductRepository productRepository, ICompanyService companyService, CompanyRights companyRights,
            IRegionService regionService, IRegionRepository regionRepository) : base(company, productRepository, companyService, companyRights, regionService, regionRepository)
        {
            Infrastructure = string.Format("{0:0.000}", company.Region.Development);
            if (ProduceAmount > 0)
                ProduceAmount = (double)Math.Round(companyService.CalculateCreatedDevelopementForCreatedItems((decimal)ProduceAmount), 4);

            foreach (var emp in Employees.Employees)
            {
                if(emp.Production.HasValue)
                emp.Production = (double)Math.Round(companyService.CalculateCreatedDevelopementForCreatedItems((decimal)emp.Production.Value), 4);
            }
        }
    }
}