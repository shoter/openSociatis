using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Country
{
    public class IndexCountryViewModel
    {
        public CountryInfoViewModel Info { get; set; }

        public List<CountryCompanyOnViewViewModel> Companies { get; set; }
        public int ICompanyREpository { get; }

        public IndexCountryViewModel(Entities.Country country)
        {
            Info = new CountryInfoViewModel(country);

            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            var companies = companyRepository.Where(company => company.OwnerID == country.ID && company.CompanyTypeID != (int)CompanyTypeEnum.Construction).ToList();
            Companies = companies.Select(company => new CountryCompanyOnViewViewModel(company)).ToList();
        }
    }
}