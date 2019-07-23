using Entities;
using Sociatis.Models.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebUtils.Mvc;

namespace Sociatis.Models.CompanyFinances
{
    public class FinanceOverviewViewModel
    {
        public CompanyInfoViewModel Info { get; set; }
        public CustomSelectList Currencies { get; set; }

        public FinanceOverviewViewModel(Company company)
        {
            Info = new CompanyInfoViewModel(company);

            var currencies = Persistent.Currencies.GetAll();

            Currencies = new CustomSelectList()
                .Add(null, "All currencies", true)
                .AddItems(currencies);
        }
    }
}
