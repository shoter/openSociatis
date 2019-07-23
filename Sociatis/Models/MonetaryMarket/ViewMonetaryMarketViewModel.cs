using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.MonetaryMarket
{
    public class ViewMonetaryMarketViewModel
    {
        public MonetaryInfoViewModel Info { get; set; }
        public List<SelectListItem> Currencies { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OfferTypes { get; set; } = new List<SelectListItem>();

        public int? BuyCurrencyID { get; set; }
        public int? SellCurrencyID  { get; set; }
        [Required]
        public int? Amount { get; set; }
        [Required]
        public double? Rate { get; set; }


        public ViewMonetaryMarketViewModel()
        {
            Info = new MonetaryInfoViewModel();

            foreach (var currency in Persistent.Currencies.GetAll())
            {
                Currencies.Add(new SelectListItem()
                {
                    Value = currency.ID.ToString(),
                    Text = currency.Symbol
                });
            }

            foreach (MonetaryOfferTypeEnum offerType in Enum.GetValues(typeof(MonetaryOfferTypeEnum)))
            {
                OfferTypes.Add(new SelectListItem()
                {
                    Text = offerType.ToString(),
                    Value = ((int)offerType).ToString()
                });
            }
        }

        public ViewMonetaryMarketViewModel(int sellCurrencyID, int buyCurrencyID) : this()
        {
            BuyCurrencyID = buyCurrencyID;
            SellCurrencyID = sellCurrencyID;
        }
    }
}