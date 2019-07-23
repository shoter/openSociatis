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
    public interface IHouseChestService
    {
        HouseChestItem GetChestItem(HouseChest chest, ProductTypeEnum productType, int quality);
        bool HasItem(HouseChest chest, ProductTypeEnum productType, int quality, int quantity = 1);

        void RemoveItem(HouseChest chest, ProductTypeEnum productType, int quality, int quantity = 1);
        void RemoveItem(HouseChestItem item, int quantity);
        void GiveItem(HouseChest chest, ProductTypeEnum productType, int quality, int quantity = 1);
        int GetUnusedSpace(HouseChest chest);

        MethodResult CanTransferItemToCitizen(Entity currentEntity, HouseChestItem item, Citizen citizen, int quantity);
        void TransferItemToCitizen(Entity currentEntity, HouseChestItem item, Citizen citizen, int quantity);

        MethodResult CanTransferItemToChest(Entity currentEntity, EquipmentItem item, HouseChest chest, int quantity);
        void TransferItemToChest(Entity currentEntity, EquipmentItem item, HouseChest chest, int quantity);

        MethodResult CanDropItem(HouseChestItem item, int amount);


    }
}
