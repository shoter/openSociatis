using Entities;
using Entities.Models.Finances;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis.Models.CompanyFinances
{
    public class DaySummaryViewModel
    {
        public class DaySummaryForCurrencyViewModel
        {
            public string CurrencySymbol { get; set; }
            public CompanyFinance Finance { get; set; }

            public DaySummaryForCurrencyViewModel(CompanyFinanceSummary summary)
            {
                var currency = Persistent.Currencies.GetById(summary.CurrencyID);
                CurrencySymbol = currency.Symbol;
                Finance = new CompanyFinance(summary);
            }
        }

        public CompanyInfoViewModel Info { get; set; }
        public IEnumerable<DaySummaryForCurrencyViewModel> CurrencySummaries { get; set; }
        public int Day { get; set; }

        public DaySummaryViewModel(Company company, IEnumerable<CompanyFinanceSummary> summaries, int day)
        {
            Day = day;
            Info = new CompanyInfoViewModel(company);

            CurrencySummaries = summaries.Select(s => new DaySummaryForCurrencyViewModel(s));
        }

        public string getColor(decimal value)
        {
            if (value > 0)
                return "green";
            if (value < 0)
                return "red";
            return "";
        }
    }
}
