using Entities;
using Sociatis.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Organisation
{
    public class OrganisationWalletViewModel : WalletViewModel
    {
        public OrganisationInfoViewModel Info { get; set; }

        public OrganisationWalletViewModel(Entities.Organisation organisation, List<WalletMoney> money) : base(money)
        {
            Info = new OrganisationInfoViewModel(organisation);
        }


    }
}