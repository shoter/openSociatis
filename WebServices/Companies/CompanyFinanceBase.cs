using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;

namespace WebServices.Companies
{
    public abstract class CompanyFinanceBase : ICompanyFinance
    {
        public abstract CompanyFinanceTypeEnum FinanceType { get; }
        public decimal Total { get; private set; }
        public int CurrencyID { get; private set; }

        public CompanyFinanceBase(decimal total, int currencyID)
        {
            Total = total;
            CurrencyID = currencyID;
        }

        public abstract void Modify(CompanyFinanceSummary summary);
    }
}
