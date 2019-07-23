using Common.Extensions;
using Common.Language;
using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Groups;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices.structs;

namespace WebServices
{
    public class EquipmentService : BaseService, IEquipmentService
    {
        private IProductService productService;
        private IEquipmentRepository equipmentRepository;
        private IEquipmentItemRepository equipmentItemRepository;

        public EquipmentService(IProductService productService, IEquipmentRepository equipmentRepository, IEquipmentItemRepository equipmentItemRepository)
        {
            this.productService = Attach(productService);
            this.equipmentRepository = equipmentRepository;
            this.equipmentItemRepository = equipmentItemRepository;
        }

        public int GetUnusedInventorySpace(Equipment equipment)
        {
            var totalSpace = equipment.ItemCapacity;
            var usedSpace = equipmentItemRepository.GetItemsForEquipment(equipment.ID)
                .Sum(item => (int?)item.Amount) ?? 0;

            return totalSpace - usedSpace;
        }

        public List<ProductTypeEnum> GetAllowedProductsForEntity(EntityTypeEnum entityType)
        {
            switch (entityType)
            {
                case EntityTypeEnum.Citizen:
                    return ProductGroups.Consumables
                        .AddRet(ProductTypeEnum.House)
                        .ToList();
                case EntityTypeEnum.Newspaper:
                    return new List<ProductTypeEnum>()
                    {
                        ProductTypeEnum.Paper,
                        ProductTypeEnum.Oil
                    };
                case EntityTypeEnum.Company:
                    {
                        return ProductGroups.All;
                    }
                case EntityTypeEnum.Organisation:
                    {
                        return ProductGroups.All;
                    }
                case EntityTypeEnum.Country:
                    {
                        return new List<ProductTypeEnum>();
                    }
                case EntityTypeEnum.Party:
                    {
                        return new List<ProductTypeEnum>();
                    }
                case EntityTypeEnum.Hotel:
                    {
                        return new List<ProductTypeEnum>()
                    {
                        ProductTypeEnum.ConstructionMaterials,
                        ProductTypeEnum.Fuel
                    };
                    }
            }

            throw new NotImplementedException();
        }



        public MethodResult<bool> CanRemoveProductsFromEquipment(EquipmentItem item, Equipment equipment)
        {
            if (item.EquipmentID != equipment.ID)
                throw new Exception("Fatal Exception CanRemoveProductsFromEquipment");

            return CanRemoveProductsFromEquipment((ProductTypeEnum)item.ProductID, item.Amount, item.Quality, equipment);
        }

        public MethodResult<bool> CanRemoveProductsFromEquipment(ProductTypeEnum productType, int amount, int quality, Equipment equipment)
        {
            var eqProduct = equipmentRepository.GetEquipmentItem(equipment.ID, (int)productType, quality);

            if (eqProduct.Amount < amount)
                return new MethodResult<bool>("There is not enough items in inventory");

            return MethodResult<bool>.Success;
        }

        public MethodResult<bool> CanHaveAccessToEquipment(Equipment equipment, Entity currentEntity, Citizen loggedCitizen)
        {
            var eqEntity = equipment.Entities.First();

            if (eqEntity.EntityID == currentEntity.EntityID)
                return true;

            if (currentEntity.GetEntityType() == EntityTypeEnum.Citizen)
            {
                switch (eqEntity.GetEntityType())
                {
                    case EntityTypeEnum.Company:
                        {
                            var companyService = DependencyResolver.Current.GetService<ICompanyService>();
                            var rights = companyService.GetCompanyRights(eqEntity.Company, currentEntity, loggedCitizen);
                            return rights.CanManageEquipment;
                        }
                    case EntityTypeEnum.Newspaper:
                        {
                            return eqEntity.Newspaper.OwnerID == currentEntity.EntityID;
                        }
                    case EntityTypeEnum.Organisation:
                        {
                            return eqEntity.Organisation.OwnerID == currentEntity.EntityID;
                        }
                    default:
                        throw new NotImplementedException();
                }
            }

            return new MethodResult<bool>("You do not have access to this inventory");
        }



        public void RemoveProductsFromEquipment(ProductTypeEnum productType, int amount, int quality, Equipment equipment)
        {
            equipmentRepository.RemoveEquipmentItem(equipment.ID, (int)productType, quality, amount);
            ConditionalSaveChanges(equipmentRepository);
        }

        public void RemoveItemFromEquipment(EquipmentItem item)
        {
            equipmentRepository.RemoveEquipmentItem(item.ID);
            ConditionalSaveChanges(equipmentRepository);
        }


        public List<ProductTypeEnum> GetAllowedProductsForCompany(Company company)
        {
            //every company needs oil
            List<ProductTypeEnum> products = new List<ProductTypeEnum>() { ProductTypeEnum.Fuel };

            switch (company.GetCompanyType())
            {
                case CompanyTypeEnum.Shop:
                    {
                        products.AddRange(ProductGroups.Consumables);
                        break;
                    }
                case CompanyTypeEnum.Producer:
                    {
                        products.Add(company.GetProducedProductType());
                        break;
                    }
                case CompanyTypeEnum.Manufacturer:
                    {
                        products.Add(company.GetProducedProductType());
                        products.Add(ProductTypeEnum.UpgradePoints);
                        products.AddRange(productService.GetRequiredProductTypes(company.GetProducedProductType()));
                        break;
                    }
                case CompanyTypeEnum.Developmenter:
                    {
                        products.Add(ProductTypeEnum.UpgradePoints);
                        products.AddRange(productService.GetRequiredProductTypes(company.GetProducedProductType()));
                        break;
                    }
                case CompanyTypeEnum.Construction:
                    {
                        products.Add(ProductTypeEnum.ConstructionMaterials);
                        break;
                    }
            }

            return products;
        }

        public bool IsAllowedItemFor(Entity entity, ProductTypeEnum productType)
        {
            var allowed = GetAllowedProductsForEntity(entity.GetEntityType());
            if (allowed.Contains(productType) == false)
                return false;
            if (entity.GetEntityType() == EntityTypeEnum.Company)
            {
                allowed = GetAllowedProductsForCompany(entity.Company);
                if (allowed.Contains(productType) == false)
                    return false;
            }
            return true;
        }

        public MethodResult CanGiveItem(Equipment equipment, ProductTypeEnum productType, int amount, int quality)
        {
            if (GetUnusedInventorySpace(equipment) < amount)
                return new MethodResult("You do not have enough free space in inventory");

            var entity = equipment.Entities.First();

            if (GetAllowedProductsForEntity((EntityTypeEnum)entity.EntityTypeID).Contains(productType) == false)
                return new MethodResult("You cannot have this kind of item");

            if ((EntityTypeEnum)entity.EntityTypeID == EntityTypeEnum.Company)
                if (GetAllowedProductsForCompany(entity.Company).Contains(productType) == false)
                    return new MethodResult("Your company cannot have this kind of item");

            return MethodResult.Success;
        }

        public MethodResult<bool> CanUseEquipmentItem(EquipmentItem item, Entity entity)
        {
            if (entity.GetEntityType() != EntityTypeEnum.Citizen)
                return new MethodResult<bool>("You are not a citizen. You cannot use items!");

            if (item.GetProductType() == ProductTypeEnum.MovingTicket)
                return true;

            if (item.GetProductType() == ProductTypeEnum.Tea)
            {
                if (entity.Citizen.DrankTeas < 10)
                    return true;
                return new MethodResult<bool>("You cannot drink more than 10 teas per day!");
            }

            return new MethodResult<bool>("You cannot use this item!");
        }

        public void GiveItem(ProductTypeEnum productType, int amount, int quality, Equipment equipment)
        {
            equipmentRepository.AddEquipmentItem(equipment.ID,
                (int)productType,
                quality,
                amount);

            ConditionalSaveChanges(equipmentRepository);
        }

        public MethodResult<string> UseEquipmentItem(EquipmentItem item, Entity entity)
        {
            switch (item.GetProductType())
            {
                case ProductTypeEnum.Tea:
                    {
                        var hpHealed = productService.GetTeaHealedAmount(item.Quality);
                        var prevHp = entity.Citizen.HitPoints;

                        entity.Citizen.HitPoints = Math.Min(100, prevHp + hpHealed);
                        entity.Citizen.DrankTeas++;

                        equipmentRepository.RemoveEquipmentItem(item.EquipmentID, item.ProductID, item.Quality);
                        equipmentRepository.SaveChanges();

                        hpHealed = entity.Citizen.HitPoints - prevHp;
                        return $"You drank tea and healed {hpHealed} hitpoint{PluralHelper.S(hpHealed)}.";
                    }
                default:
                    throw new Exception("No action defined for " + EquipmentItemExtension.GetProductType(item).ToString());
            }
        }

        public MethodResult HaveItem(Equipment equipment, ProductTypeEnum productType, int quality, int amount)
        {
            if (equipment == null)
                return new MethodResult("Equipment does not exist!");

            if (productType.IsDefined() == false)
                return new MethodResult("Wrong product type!");

            var item = equipmentRepository.GetEquipmentItem(equipment.ID, productType.ToInt(), quality);

            if (item == null || item.Amount < amount)
                return new MethodResult("You does not have this item!");

            return MethodResult.Success;
        }
    }
}
