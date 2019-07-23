using Sociatis.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace Sociatis.Models.Companies
{
    public class CompanyWalletViewModel : WalletViewModel
    {
        public CompanyInfoViewModel Info { get; set; }

        public CompanyWalletViewModel(Company company, List<WalletMoney> money) : base(money)
        {
            Info = new CompanyInfoViewModel(company);
        }

        
    }
}