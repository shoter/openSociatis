using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SociatisCommon.Errors
{
    public enum MarketOfferErrors
    {
        [Description("Offer does not exist!")]
        OfferNotExist,
        [Description("You cannot buy it from this company!")]
        WrongCompany,
        [Description("You do not have enough money to buy it!")]
        NotEnoughMoney,
        [Description("You do not have enough fuel to buy it!")]
        NotEngouhFuel,
        [Description("Offer has less products than amount you want to buy!")]
        NotEnoughProducts,
        [Description("You cannot have this product!")]
        NotAllowedProduct,
        [Description("You cannot buy that offer!")]
        CannotBuy,
    }
}
