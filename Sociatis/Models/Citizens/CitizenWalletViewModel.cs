using Sociatis.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using WebServices;

namespace Sociatis.Models.Citizens
{
    public class CitizenWalletViewModel : WalletViewModel
    {
        public CitizenInfoViewModel Info { get; set; }
        public CitizenWalletViewModel(Citizen citizen, List<WalletMoney> money, IFriendService friendService) : base(money)
        {
            Info = new CitizenInfoViewModel(citizen, friendService);
        }
    }
}