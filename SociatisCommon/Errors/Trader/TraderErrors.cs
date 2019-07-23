using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SociatisCommon.Errors.Trader
{
    public enum TraderErrors
    {
        [Description("You cannot buy that!")]
        YouCannotBuyThat,
        [Description("You cannot have that!")]
        YouCannotHaveThat,
        [Description("This offer is not selled in your actual country!")]
        NotSelledInYourCountry,
        [Description("Offer is not selled in your region!")]
        NotSelledInYourRegion
    }
}
