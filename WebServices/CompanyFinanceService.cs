using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;

namespace WebServices
{
    public class CompanyFinanceService : ICompanyFinanceService
    {
        private readonly ICompanyService companyService;

        public CompanyFinanceService(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        public MethodResult CanAccessFinances(Company company, Entity entity, Citizen loggedCitizen)
        {
            if (company == null)
                return new MethodResult("Company does not exist!");
            if (entity == null)
                return new MethodResult("You do not exist O_o");

            var rights = companyService.GetCompanyRights(company, entity, loggedCitizen);

            if (rights.HaveAnyRights == false)
                return new MethodResult("You cannot access company's finances!");

            return MethodResult.Success;
        }
    }
}
