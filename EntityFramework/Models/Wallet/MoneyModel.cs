using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Wallet
{
    public class MoneyModel
    {
        public int CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public string Symbol { get; set; }
    }
}
