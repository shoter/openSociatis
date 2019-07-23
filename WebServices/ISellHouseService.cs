using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ISellHouseService
    {
        MethodResult CanBuy(House house, Entity entity);
        void Buy(House house, Entity entity);

        MethodResult CanSellHouse(House house, decimal price);
        void SellHouse(House house, decimal price);
    }
}
