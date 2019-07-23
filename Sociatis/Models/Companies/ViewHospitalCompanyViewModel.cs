using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using Entities.Repository;
using WebServices;
using WebServices.structs;
using Sociatis.Helpers;
using Entities.Extensions;
using Entities.enums;
using Sociatis.Models.Hospitals;

namespace Sociatis.Models.Companies
{
    public class ViewHospitalCompanyViewModel : ViewCompanyViewModel
    {
        public HospitalHealViewModel Heal { get; set; }

        public ViewHospitalCompanyViewModel(Company company, IProductRepository productRepository, ICompanyService companyService, IHospitalService hospitalService, CompanyRights companyRights,
            IRegionService regionService, IRegionRepository regionRepository, IHospitalRepository hospitalRepository) : base(company, productRepository, companyService, companyRights, regionService, regionRepository)
        {
            var hospital = company.Hospital;
            Heal = new HospitalHealViewModel(hospital, hospitalService, hospitalRepository);

        }
    }
}