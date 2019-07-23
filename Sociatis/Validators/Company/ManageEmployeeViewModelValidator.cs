using Entities.Repository;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Validators.Company
{
    public class ManageEmployeeViewModelValidator : Validator<ManageEmployeeViewModel>
    {
        ICompanyEmployeeRepository companyEmployeeRepository;

        public ManageEmployeeViewModelValidator(ModelStateDictionary ModelState, ICompanyEmployeeRepository companyEmployeeRepository) : base(ModelState)
        {
            this.companyEmployeeRepository = companyEmployeeRepository;
        }

        public bool Validate(ManageEmployeeViewModel model)
        {
            var entity = companyEmployeeRepository.GetById(model.CitizenID);
            var contract = entity.JobContract;

            if(model.Salary < (double)contract.MinSalary)
            {
                AddError("Salary is lower than contract's minimum salary", () => model.Salary);
            }

            if(model.MinimumHP < (double)contract.MinHP)
            {
                AddError("HP is lower than contract's minimum HP", () => model.MinimumHP);
            }

            return IsValid;
        }
    }
}