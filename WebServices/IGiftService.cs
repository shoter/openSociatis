using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IGiftService
    {
        MethodResult CanSendMoneyGift(Entity source, Entity destination, Currency currency, decimal amount);
        MethodResult CanSendProductGift(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount);

        void SendMoneyGift(Entity source, Entity destination, Currency currency, decimal amount);
        void SendProductGift(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount);

        int GetNeededFuelToSendGift(Entity source, Entity destination, ProductTypeEnum productType, int quality, int amount);

        bool WillGiftUseFuel(Entity source, Entity destination);

        MethodResult CanReceiveProductGifts(Entity source, Entity destination);
    }
}
