using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CompanyFinanceSummaryRepository : RepositoryBase<CompanyFinanceSummary, SociatisEntities>, ICompanyFinanceSummaryRepository
    {
        public CompanyFinanceSummaryRepository(SociatisEntities context) : base(context)
        {
        }

        public CompanyFinanceSummary GetOrAdd(int companyID, int day, int currencyID)
        {
            var summary = FirstOrDefault(s => s.CompanyID == companyID
            && s.Day == day
            && s.CurrencyID == currencyID);

            if (summary == null)
            {
                summary = new CompanyFinanceSummary()
                {
                    CurrencyID = currencyID,
                    Day = day,
                    CompanyID = companyID
                };
                Add(summary);
            }

            return summary;
        }

        public List<CompanyFinanceSummary> GetOrAdd(int companyID, int day, params int[] currenciesIDs)
        {
            var summaries = Where(s => s.CompanyID == companyID
            && s.Day == day
            && currenciesIDs.Contains(s.CurrencyID))
            .ToList();

            foreach (var c in currenciesIDs)
            {
                if (summaries.Any(s => s.CurrencyID == c))
                    continue;

                var summary = new CompanyFinanceSummary()
                {
                    CompanyID = companyID,
                    Day = day,
                    CurrencyID = c
                };
                summaries.Add(summary);

                Add(summary);
            }

            return summaries;
        }

        public List<CompanyFinanceSummary> GetSummariesForDay(int companyID, int day)
        {
            return Where(s => s.CompanyID == companyID && s.Day == day)
                .ToList();
        }
    }
}
