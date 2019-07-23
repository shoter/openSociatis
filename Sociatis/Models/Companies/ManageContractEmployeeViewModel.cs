using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using WebServices.structs;
using System.ComponentModel;

namespace Sociatis.Models.Companies
{
    public class ManageContractEmployeeViewModel : ManageEmployeeViewModel
    {
        [DisplayName("Remaining contract length")]
        public int RemainingLength { get; set; }
        [DisplayName("Minimum salary")]
        public double MinimumSalary { get; set; }
        [DisplayName("Minimum HP on contract")]
        public double MinimumHPOnContract { get; set; }

        [DisplayName("Signed by")]
        public string SignedByName { get; set; }
        public bool AbusedCompanyRules { get; set; }
        public ManageContractEmployeeViewModel() { }
        public ManageContractEmployeeViewModel(CompanyEmployee employee, CompanyRights managerRights, bool isEmployeeManager) : base(employee, managerRights, isEmployeeManager)
        {
            var contract = employee.JobContract;

            RemainingLength = contract.Length;
            MinimumSalary = (double)contract.MinSalary;
            MinimumHPOnContract = contract.MinHP;
            SignedByName = contract.Entity.Name;
            AbusedCompanyRules = contract.AbusedByEmployee;

        }
    }
}