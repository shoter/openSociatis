using Entities.structs.MonetaryMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.MonetaryMarket
{
    public class ViewReservedViewModel
    {
        public MonetaryInfoViewModel Info { get; set; }
        public List<ReservedMoneyViewModel> Reserved { get; set; } = new List<ReservedMoneyViewModel>();
        public ViewReservedViewModel(Dictionary<int, ReservedMoney> reserved)
        {
            Info = new MonetaryInfoViewModel();

            foreach(var res in reserved)
            {
                var currency = Persistent.Currencies.GetById(res.Key);

                var vm = new ReservedMoneyViewModel(res.Value, currency);

                Reserved.Add(vm);
            }

        }
    }
}