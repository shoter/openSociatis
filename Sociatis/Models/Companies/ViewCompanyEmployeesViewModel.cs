using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ViewCompanyEmployeesViewModel
    {
        public bool ShowEmployeeStats { get; set; }
        public CompanyRights Rights { get; set; }
        public List<CompanyEmployeeViewModel> Employees { get; set; }

        public ViewCompanyEmployeesViewModel(List<CompanyEmployeeViewModel> employees, bool showEmployeeStats, CompanyRights rights)
        {
            ShowEmployeeStats = showEmployeeStats;
            Rights = rights;

            Employees = employees;
        }
    }
}