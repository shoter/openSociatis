using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Extensions;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;

namespace WebServices
{
    public class HouseChestService : BaseService, IHouseChestService
    {
        private readonly IHouseChestItemRepository houseChestItemRepository;
        private readonly IEquipmentService equipmentService;

        public HouseChestService(IHouseChestItemRepository houseChestItemRepository, IEquipmentService equipmentService)
        {
            this.houseChestItemRepository = houseChestItemRepository;
            this.equipmentService = equipmentService;
        }
        public HouseChestItem GetChestItem(HouseChest chest, ProductTypeEnum productType, int quality)
        {
            return chest
                .HouseChestItems
                .Where(item => item.ProductID == (int)productType && item.Quality == quality)
                .FirstOrDefault();
        }

        public bool HasItem(HouseChest chest, ProductTypeEnum productType, int quality, int quantity = 1)
        {
            var item = GetChestItem(chest, productType, quality);

            if (item == null)
                return false;
            return item.Quantity >= quantity;
        }

        public void RemoveItem(HouseChest chest, ProductTypeEnum productType, int quality, int quantity = 1)
        {
            var item = GetChestItem(chest, productType, quality);

            item.Quantity -= quantity;
            if (item.Quantity < 0)
                throw new Exception($"Quantity in chest({chest.FurnitureID}) is less than 0!");

            if (item.Quantity == 0)
                houseChestItemRepository.Remove(item);
            ConditionalSaveChanges(houseChestItemRepository);
        }

        public void RemoveItem(HouseChestItem item, int quantity)
        {
            RemoveItem(item.HouseChest, (ProductTypeEnum)item.ProductID, item.Quality, quantity);
        }

        public void GiveItem(HouseChest chest, ProductTypeEnum productType, int quality, int quantity = 1)
        {
            var item = GetChestItem(chest, productType, quality);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                item = new HouseChestItem()
                {
                    ChestID = chest.FurnitureID,
                    ProductID = (int)productType,
                    Quality = quality,
                    Quantity = quantity
                };

                houseChestItemRepository.Add(item);
            }
            ConditionalSaveChanges(houseChestItemRepository);
        }


        public int GetUnusedSpace(HouseChest chest)
        {
            int capacity = chest.Capacity;
            int itemCount = chest.GetItemCount();

            return capacity - itemCount;
        }


        public MethodResult CanTransferItemToCitizen(Entity currentEntity, HouseChestItem item, Citizen citizen, int quantity)
        {
            
            if (item == null)
                return new MethodResult("Item does not exist!");

            if (citizen == null)
                return new MethodResult("Citizen does not exist!");

            if (currentEntity.EntityID != citizen.ID)
                return new MethodResult("You must be a citizen owning this chest to do that!");

            if (quantity <= 0)
                return new MethodResult("You must transfer at least 1 item!");

            ProductTypeEnum productType = (ProductTypeEnum)item.ProductID;
            if (item.Quantity < quantity)
                return new MethodResult($"You do not have enough {productType.ToHumanReadable().FirstUpper()} in chest!");

            var equipment = citizen.Entity.Equipment;

            if (equipmentService.GetUnusedInventorySpace(equipment) < quantity)
                return new MethodResult("You do not have enough space in inventory!");

            return MethodResult.Success;
        }

        public void TransferItemToCitizen(Entity currentEntity, HouseChestItem item, Citizen citizen, int quantity)
        {
            var equipment = citizen.Entity.Equipment;

            equipmentService.GiveItem((ProductTypeEnum)item.ProductID, quantity, item.Quality, equipment);
            RemoveItem(item, quantity);
        }

        public MethodResult CanTransferItemToChest(Entity currentEntity, EquipmentItem item, HouseChest chest, int quantity)
        {
            if (currentEntity.EquipmentID != item.EquipmentID)
                return new MethodResult("You cannot transfer items from your chest to someone else!");

            if (item == null)
                return new MethodResult("Item does not exist!");

            if (quantity <= 0)
                return new MethodResult("You must transfer at least 1 item!");

            ProductTypeEnum productType = (ProductTypeEnum)item.ProductID;
            if (item.Amount < quantity)
                return new MethodResult($"You do not have enough {productType.ToHumanReadable().FirstUpper()} in chest!");

            if(GetUnusedSpace(chest) < quantity)
                return new MethodResult("You do not have enough space in chest!");

            return MethodResult.Success;
        }

        public void TransferItemToChest(Entity currentEntity, EquipmentItem item, HouseChest chest, int quantity)
        {
            equipmentService.RemoveProductsFromEquipment((ProductTypeEnum)item.ProductID, quantity, item.Quality, item.Equipment);
            GiveItem(chest, (ProductTypeEnum)item.ProductID, item.Quality, quantity);
        }

        public MethodResult CanDropItem(HouseChestItem item, int amount)
        {
            if (amount <= 0)
                return new MethodResult("You must drop at least 1 item!");

            if (item == null)
                return new MethodResult("Item does not exist!");

            if (item.Quantity < amount)
                return new MethodResult("You cannot drop more than you have!");

            return MethodResult.Success;
        }



    }
}
