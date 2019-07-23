using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public static class CompanyViewModelChooser
    {
        public static ViewCompanyViewModel GetViewModel(Company company, IProductRepository productRepository, ICompanyService companyService, CompanyRights rights, IRegionRepository regionRepository, IRegionService regionService, IHospitalRepository hospitalRepository)
        {
            switch (company.GetCompanyType())
            {
                case CompanyTypeEnum.Construction:
                    return new ViewConstructionCompanyViewModel(company, productRepository, companyService, rights, regionService, regionRepository);
                case CompanyTypeEnum.Developmenter:
                    return new ViewDevelopementCompanyViewModel(company, productRepository, companyService, rights, regionService, regionRepository);
            }

            switch (company.GetProducedProductType())
            {
                case ProductTypeEnum.MedicalSupplies:
                    var hospitalService = DependencyResolver.Current.GetService<IHospitalService>();
                    return new ViewHospitalCompanyViewModel(company, productRepository, companyService, hospitalService, rights, regionService, regionRepository, hospitalRepository);
            }

            return new ViewCompanyViewModel(company, productRepository, companyService, rights, regionService, regionRepository); 
        }

    }
}