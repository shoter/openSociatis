using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.MonetaryMarket
{
    public class MonetaryTransactionsViewModel
    {
        public MonetaryInfoViewModel Info { get; set; }
        public int EntityID { get; set; }

        public MonetaryTransactionsViewModel(Entity entity)
        {
            EntityID = entity.EntityID;

            Info = new MonetaryInfoViewModel();
        }
    }
}