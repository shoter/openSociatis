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
    public class ContractService : BaseService, IContractService
    {
        IContractRepository contractRepository;
        ICompanyEmployeeRepository companyEmployeeRepository;
        IWarningService warningService;
        
        public ContractService(IContractRepository contractRepository, ICompanyEmployeeRepository companyEmployeeRepository, IWarningService warningService)
        {
            this.contractRepository = contractRepository;
            this.companyEmployeeRepository = companyEmployeeRepository;
            this.warningService = Attach(warningService);
        }

        private void terminateContract(CompanyEmployee employee, JobContract contract)
        {
            contractRepository.Remove(contract.ID);
            employee.JobContractID = null;
            ConditionalSaveChanges(contractRepository);

            warningService.AddWarning(employee.CitizenID, "Your contract has ended.");
        }

        public void ProcessDayChange()
        {
            var contracts = contractRepository.GetAll();
            using (NoSaveChanges)
            {
                foreach (var contract in contracts)
                {
                    contract.Length--;
                    
                    var employee = contract.CompanyEmployees.First();
                    if (employee.Citizen.Worked == false)
                    {
                        AddEmployeeAbusement(employee);

                        var company = employee.Company;
                        var citizen = employee.Citizen;


                        var msg = string.Format("Contract worker {0} was not working today in {1}. You are free to fire him from work.", citizen.Entity.Name, company.Entity.Name);
                        warningService.AddWarning(company.ID, msg);
                    }
                    if (contract.Length <= 0)
                    {
                        terminateContract(employee, contract);
                    }
                }
            }
            ConditionalSaveChanges(contractRepository);
        }

        /// <summary>
        /// It does nothing to non-contract jobs
        /// </summary>
        /// <param name="employee"></param>
        public void AddCompanyAbusement(CompanyEmployee employee)
        {
            if (employee.GetJobType() == JobTypeEnum.Contracted)
            {
                if (employee.JobContract.AbusedByCompany == false)
                {
                    using (NoSaveChanges)
                    {
                        InformatAboutCompanyAbusement(employee.Company, employee);
                    }
                }
                employee.JobContract.AbusedByCompany = true;
                ConditionalSaveChanges(companyEmployeeRepository);
            }
        }

        public void InformatAboutCompanyAbusement(Company company, CompanyEmployee employee)
        {
            var employeeName = employee.Citizen.Entity.Name;

            string employeeMsg = string.Format("You have abused your contract in {0}. Company boss may fire you from any moment right now.", company.Entity.Name);
            string companyMsg = string.Format("{0} has abused your contract. You may fire him from now on", employeeName);

            warningService.AddWarning(employee.CitizenID, employeeMsg);
            warningService.AddWarning(company.ID, companyMsg);
        }

        /// <summary>
        /// It does nothing to non-contract jobs
        /// </summary>
        /// <param name="employee"></param>
        public void AddEmployeeAbusement(CompanyEmployee employee)
        {
            if (employee.GetJobType() == JobTypeEnum.Contracted)
            {
                if(employee.JobContract.AbusedByEmployee == false)
                {
                    using (NoSaveChanges)
                    {
                        InformAboutEmployeeAbusement(employee.Company, employee);
                    }
                }
                employee.JobContract.AbusedByEmployee = true;
                ConditionalSaveChanges(companyEmployeeRepository);
            }
        }

        public void InformAboutEmployeeAbusement(Company company, CompanyEmployee employee)
        {
            var employeeName = employee.Citizen.Entity.Name;

            string employeeMsg = string.Format("{0} has abused your contract rights. You may from now on resign from your job.", company.Entity.Name);
            string companyMsg = string.Format("Company abused {0}'s contract. {0} may leave work from now in any moment.", employeeName);

            warningService.AddWarning(employee.CitizenID, employeeMsg);
            warningService.AddWarning(company.ID, companyMsg);
        }
    }
}
