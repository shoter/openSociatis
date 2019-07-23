using Entities;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ManageEmployeeViewModel
    {
        public CompanyInfoViewModel CompanyInfo { get; set; }
        public int CitizenID { get; set; }
        public string CitizenName { get; set; }
        public ImageViewModel Avatar { get; set; }
        public double Skill { get; set; }

        public double Salary { get; set; }
        public ImageViewModel CurrencyImage { get; set; }
        [DisplayName("Minimum hitpoints to work")]
        [Range(0, 100)]
        public int MinimumHP { get; set; }

        public bool IsManager { get; set; }

        public CompanyRights ManagerRights { get; set; }

        public ManageEmployeeViewModel() { }
        public ManageEmployeeViewModel(CompanyEmployee employee, CompanyRights managerRights, bool isEmployeeManager)
        {
            CompanyInfo = new CompanyInfoViewModel(employee.Company);

            IsManager = isEmployeeManager;

            CitizenID = employee.CitizenID;
            CitizenName = employee.Citizen.Entity.Name;
            Avatar = new ImageViewModel(employee.Citizen.Entity.ImgUrl);
            MinimumHP = employee.MinHP;
            Salary = (double)employee.Salary;


            var countryID = employee.Company.Region.CountryID;

            var currency = Persistent.Countries
                .First(c => c.ID == countryID)
                .Currency;

            CurrencyImage = Images.GetCountryCurrency(currency).VM;

            ManagerRights = managerRights;

            var companyService = DependencyResolver.Current.GetService<ICompanyService>();
            Skill = employee.Citizen.GetWorkSkill(employee.Company); 

        }
    }
}