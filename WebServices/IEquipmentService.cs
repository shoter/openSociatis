using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;

namespace WebServices
{
    public interface IEquipmentService
    {
        MethodResult<bool> CanUseEquipmentItem(EquipmentItem item, Entity entity);
        /// <returns>information to display about used item.</returns>
        MethodResult<string> UseEquipmentItem(EquipmentItem item, Entity entity);

        /// <summary>
        /// It will not create new instance of item in inventory if there is existing one. Instead it will add amount to existing product.
        /// </summary>
        void GiveItem(ProductTypeEnum productType, int amount, int quality, Equipment equipment);
        int GetUnusedInventorySpace(Equipment equipment);

        List<ProductTypeEnum> GetAllowedProductsForEntity(EntityTypeEnum entityType);
        List<ProductTypeEnum> GetAllowedProductsForCompany(Company company);
        MethodResult CanGiveItem(Equipment equipment, ProductTypeEnum productType, int amount, int quality);

        MethodResult<bool> CanRemoveProductsFromEquipment(ProductTypeEnum productType, int amount, int quality, Equipment equipment);
        void RemoveProductsFromEquipment(ProductTypeEnum productType, int amount, int quality, Equipment equipment);
        MethodResult<bool> CanRemoveProductsFromEquipment(EquipmentItem item, Equipment equipment);
        void RemoveItemFromEquipment(EquipmentItem item);

        MethodResult<bool> CanHaveAccessToEquipment(Equipment equipment, Entity currentEntity, Citizen loggedCitizen);

        MethodResult HaveItem(Equipment equipment, ProductTypeEnum productType, int quality, int amount);

        bool IsAllowedItemFor(Entity entity, ProductTypeEnum productType);



    }
}
