using Entities;
using Entities.Models.Hotels;
using Sociatis.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Hotels
{
    public class HotelWalletViewModel : WalletViewModel
    {
        public HotelInfoViewModel Info { get; set; }

        public HotelWalletViewModel(HotelInfo info, List<WalletMoney> money) : base(money)
        {
            Info = new HotelInfoViewModel(info);
        }
    }
}
