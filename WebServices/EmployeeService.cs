using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Repository;
using Entities.enums;
using Entities.Extensions;

namespace WebServices
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly ICompanyEmployeeRepository employeeRepository;
        private readonly IWarningService warningService;

        public EmployeeService(ICompanyEmployeeRepository employeeRepository, IWarningService warningService)
        {
            this.employeeRepository = employeeRepository;
            this.warningService = Attach(warningService);
        }
        public void InformAboutSalaryChange(CompanyEmployee employee)
        {
            var company = employee.Company;
            var currencyID = company.Region.Country.CurrencyID;
            var currency = Persistent.Currencies.First(c => c.ID == currencyID);

            var msg = string.Format("Your salary in {0} changed to {1} {2}", company.Entity.Name, employee.Salary, currency.Symbol);

            warningService.AddWarning(employee.CitizenID, msg);
        }

        public void InformAboutMinimumHPChange(CompanyEmployee employee)
        {
            var company = employee.Company;

            var msg = string.Format("Your Minimum HP in {0} changed to {1}", company.Entity.Name, employee.MinHP);

            warningService.AddWarning(employee.CitizenID, msg);
        }

        public void InformAboutDismiss(CompanyEmployee employee, Company company)
        {
            var msg = string.Format("You were fired from {0} company", company.Entity.Name);

            warningService.AddWarning(employee.CitizenID, msg);
        }

        public void ProcessDayChange(int newDay)
        {
            var employees = employeeRepository.GetAll();

            foreach (var employee in employees)
            {
                employee.TodayProduction = null;
                employee.TodayHP = null;
                employee.TodaySalary = null;
            }

            ConditionalSaveChanges(employeeRepository);

        }

       
    }
}
