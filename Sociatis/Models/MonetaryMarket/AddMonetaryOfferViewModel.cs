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
    public class AddMonetaryOfferViewModel
    {
        [Required]
        public int? SellCurrencyID { get; set; }

        [Required]
        public int? BuyCurrencyID { get; set; }

        public int Amount { get; set; }

        public double Rate { get; set; }

        public int? OfferType { get; set; }

      

        public AddMonetaryOfferViewModel()
        {
            
        }

    }
}