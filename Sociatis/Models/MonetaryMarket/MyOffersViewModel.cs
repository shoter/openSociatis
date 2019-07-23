using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.MonetaryMarket
{
    public class MyOffersViewModel
    {
        public MonetaryInfoViewModel Info { get; set; }
        public List<MyOfferViewModel> Offers { get; set; } = new List<MyOfferViewModel>();

        public MyOffersViewModel(IEnumerable<MonetaryOffer> offers)
        {
            Info = new MonetaryInfoViewModel();
            foreach(var offer in offers)
            {
                Offers.Add(new MyOfferViewModel(offer));
            }
        }
    }
}