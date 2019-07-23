using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IEmployeeService
    {
        void InformAboutSalaryChange(CompanyEmployee employee);
        void InformAboutDismiss(CompanyEmployee employee, Company company);
        void InformAboutMinimumHPChange(CompanyEmployee employee);

        void ProcessDayChange(int newDay);
    }
}
