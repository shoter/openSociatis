using Entities;
using Entities.Models.Hospitals;
using Sociatis.Code;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Hospitals
{
    public class ManageHospitalViewModel
    {
        public CompanyInfoViewModel Info { get; set; }
        public bool HealingEnabled { get; set; }
        public decimal? HealingPrice { get; set; }

        public List<UpdateHospitalNationalityPriceViewModel> NationalityOptions { get; set; }

        public List<SelectListItem> Countries { get; set; } = SelectLists.Countries;

        public ManageHospitalViewModel(Hospital hospital, HospitalManage hospitalManage)
        {
            Info = new CompanyInfoViewModel(hospital.Company);

            HealingEnabled = hospitalManage.HealingEnabled;
            HealingPrice = hospitalManage.HealingPrice;

            NationalityOptions = hospitalManage.hospitalManageNationalityHealingOptions
                .Select(o => new UpdateHospitalNationalityPriceViewModel(o)).ToList();
        }

        public ManageHospitalViewModel()
        {

        }
    }
}