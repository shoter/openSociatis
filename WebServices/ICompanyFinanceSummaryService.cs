using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Companies;

namespace WebServices
{
    public interface ICompanyFinanceSummaryService
    {
        void AddFinances(Company company, params ICompanyFinance[] finance);
    }
}
