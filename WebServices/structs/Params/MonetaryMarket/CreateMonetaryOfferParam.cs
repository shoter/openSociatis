using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Params.Attributes;

namespace WebServices.structs.Params.MonetaryMarket
{
    public class CreateMonetaryOfferParam : BaseParam
    {
        [Required]
        public Currency SellCurency { get; set; }
        [Required]
        public Currency BuyCurrency { get; set; }
        [Required]
        public Entity Seller { get; set; }
        [Required]
        public int? Amount { get; set; }

        private double? rate;
        [Required]
        public double? Rate
        {
            get
            {
                return rate;
            }
            set
            {
                if (value.HasValue)
                    rate = Math.Round(value.Value, 4);
                else
                    rate = null;
            }
        }

        [Required]
        public MonetaryOfferTypeEnum? OfferType { get; set; }
    }
}
