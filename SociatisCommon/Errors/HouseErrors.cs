using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SociatisCommon.Errors
{
    public enum HouseErrors
    {
        [Description("House do not exist!")]
        HouseNotExist,
        [Description("Entity do not exist!")]
        EntityNotExist,
        [Description("You already have house in this region!")]
        AlreadyHaveHouse,
        [Description("You do not have enough cash!")]
        NotEnoughCash,
        [Description("Only citizens can buy houses!")]
        OnlyCitizenBuyHouse,
        [Description("House is not on sell!")]
        NotOnSell,
    }
}
