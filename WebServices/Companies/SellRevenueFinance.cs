using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;

namespace WebServices.Companies
{
    public class SellRevenueFinance : CompanyFinanceBase
    {
        public SellRevenueFinance(decimal total, int currencyID) : base(total, currencyID)
        {
        }

        public override CompanyFinanceTypeEnum FinanceType => CompanyFinanceTypeEnum.SellRevenue;

        public override void Modify(CompanyFinanceSummary summary)
        {
            Debug.Assert(summary.CurrencyID == CurrencyID);

            summary.SellRevenue += Total;
        }
    }
}
