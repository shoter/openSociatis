using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;
using WebServices.structs;

namespace WebServices
{
    public interface IHouseTransactions
    {
        TransactionResult PayForHouseBuy(Money money, Citizen owner, Citizen buyer, House house);
    }
}
