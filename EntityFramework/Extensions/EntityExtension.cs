using Entities.enums;
using Entities.Items;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class EntityExtension
    {
        public static EntityTypeEnum GetEntityType(this Entity entity)
        {
            return (EntityTypeEnum)entity.EntityTypeID;
        }

        public static Region GetCurrentRegion(this Entity entity)
        {
            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    {
                       return entity.Citizen.Region;
                    }
                case EntityTypeEnum.Company:
                    {
                        return entity.Company.Region;
                    }
                case EntityTypeEnum.Hotel:
                    {
                        return entity.Hotel.Region;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public static Country GetCurrentCountry(this Entity entity)
        {
            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                case EntityTypeEnum.Company:
                    {
                        return entity.GetCurrentRegion().Country;
                    }
                case EntityTypeEnum.Organisation:
                    {
                        return entity.Organisation.Country;
                    }
                case EntityTypeEnum.Party:
                    {
                        return entity.Party.Country;
                    }
                case EntityTypeEnum.Country:
                    {
                        return entity.Country;
                    }
                case EntityTypeEnum.Newspaper:
                    {
                        return entity.Newspaper.Country;
                    }
                case EntityTypeEnum.Hotel:
                    {
                        return entity.Hotel.Region.Country;
                    }
            }
            throw new Exception();
        }

        /// <summary>
        /// Warning. Equipment and EquipmentID property may be null when product is not found.
        /// Never returns null. Check quantity for 0. 
        /// </summary>
        /// <returns>NEVER RETURNS NULL</returns>
        public static EquipmentItem GetEquipmentItem(this Entity entity, ItemBase item, IProductRepository productRepository)
        {

            return entity.GetEquipmentItem(item.ProductType, item.Quality, productRepository);
        }
        /// <summary>
        /// Warning. Equipment and EquipmentID property may be null when product is not found.
        /// Never returns null. Check quantity for 0. 
        /// </summary>
        /// <returns>NEVER RETURNS NULL</returns>
        public static EquipmentItem GetEquipmentItem(this Entity entity, ProductTypeEnum productType, int quality, IProductRepository productRepository)
        {
            var equipment = entity.Equipment;

            var item = equipment.EquipmentItems
                .FirstOrDefault(i => i.ProductID == (int)productType && i.Quality == quality);

            if (item == null)
            {
                var product = productRepository.GetById((int)productType);
                return
                    new EquipmentItem()
                    {
                        Product = product,
                        ProductID = product.ID,
                        Quality = quality,
                        Amount = 0
                    };
            }

            return item;
        }

        public static bool Is(this Entity entity, params EntityTypeEnum[] entityType)
        {
           return entityType.Contains((EntityTypeEnum)entity.EntityTypeID);
        }

        public static bool IsNot(this Entity entity, params EntityTypeEnum[] entityType)
        {
            return entityType.Contains((EntityTypeEnum)entity.EntityTypeID) == false ;
        }

    }
}
