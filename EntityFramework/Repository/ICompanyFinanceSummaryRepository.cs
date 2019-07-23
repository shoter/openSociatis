using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICompanyFinanceSummaryRepository : IRepository<CompanyFinanceSummary>
    {
        CompanyFinanceSummary GetOrAdd(int companyID, int day, int currencyID);
        List<CompanyFinanceSummary> GetOrAdd(int companyID, int day, params int[] currencyID);

        List<CompanyFinanceSummary> GetSummariesForDay(int companyID, int day);
    }
}
