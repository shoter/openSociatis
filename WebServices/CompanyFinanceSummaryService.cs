using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Companies;
using WebServices.Helpers;

namespace WebServices
{
    public class CompanyFinanceSummaryService : BaseService, ICompanyFinanceSummaryService
    {
        private readonly ICompanyFinanceSummaryRepository companyFinanceSummaryRepository;

        public CompanyFinanceSummaryService(ICompanyFinanceSummaryRepository companyFinanceSummaryRepository)
        {
            this.companyFinanceSummaryRepository = companyFinanceSummaryRepository;
        }

        public void AddFinances(Company company, params ICompanyFinance[] finance)
        {
            int[] currencies = finance.Select(f => f.CurrencyID).Distinct().ToArray();
            var summaries = companyFinanceSummaryRepository.GetOrAdd(company.ID, GameHelper.CurrentDay, currencies);

            foreach (var f in finance)
            {
                var summary = summaries.First(s => s.CurrencyID == f.CurrencyID);
                f.Modify(summary);
            }

            companyFinanceSummaryRepository.SaveChanges();
        }
        
    }
}
