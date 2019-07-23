using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis_Test_Suite.Dummies.Companies
{
    public class CompanyFinanceSummaryDummyCreator : IDummyCreator<CompanyFinanceSummary>
    {
        private readonly Company company;
        private CompanyFinanceSummary summary;

        public CompanyFinanceSummaryDummyCreator(Company company)
        {
            this.company = company;
            create();
        }

        private void create()
        {
            var currency = new CurrencyDummyCreator().Create();
            summary = new CompanyFinanceSummary()
            {
                Day = GameHelper.CurrentDay,
                Company = company,
                CompanyID = company.ID,
                Currency = currency,
                CurrencyID = currency.ID,

            };


        }

        public CompanyFinanceSummaryDummyCreator SetCurrency(Currency currency)
        {
            summary.Currency = currency;
            summary.CurrencyID = currency.ID;

            return this;
        }

        public CompanyFinanceSummary Create()
        {
            var temp = summary;
            create();
            return temp;
        }
    }
}
