using Entities;
using Entities.Extensions;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis.Models.Contracts
{
    public class IndexContractViewModel
    {
        public CompanyInfoViewModel Info { get; set; }
        public string CompanyName { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public string EmployeeName { get; set; }
        public int ContractID { get; set; }
        public double MinimalSalary { get; set; }
        public string SalarySymbol { get; set; }
        public double MinimumHP { get; set; }
        public int StartDay { get; set; }
        public int RemainingTime { get; set; }
        public int EndDay { get; set; }
        public string SigneeName { get; set; }

        public IndexContractViewModel() { }

        public IndexContractViewModel(JobContract Contract)
        {
            

            var ce = Contract.CompanyEmployees.First();
            var company = ce.Company;
            var region = company.Entity.GetCurrentRegion();
            var country = region.Country;
            var currency = country.Currency;

            Info = new CompanyInfoViewModel(company);

            CountryName = country.Entity.Name;
            RegionName = region.Name;
            ContractID = Contract.ID;
            EmployeeName = ce.Citizen.Entity.Name;
            MinimumHP = Contract.MinHP;
            MinimalSalary = (double)Contract.MinSalary;
            CompanyName = company.Entity.Name; 
            StartDay = ce.StartDay;
            RemainingTime = Contract.Length;
            EndDay = GameHelper.CurrentDay + RemainingTime;
            SalarySymbol = currency.Symbol;
            SigneeName = Contract.Entity.Name;
        }

    }
}
