using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Companies
{
    public interface ICompanyFinance
    {
        CompanyFinanceTypeEnum FinanceType { get; }
        decimal Total { get; }
        int CurrencyID { get; }
        void Modify(CompanyFinanceSummary summary);
    }
}
