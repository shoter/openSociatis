using Entities;
using Entities.enums;
using Sociatis.Helpers;
using Sociatis.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Equipment
{
    public class EquipmentItemViewModel : ProductViewModel
    {
        public int ID { get; set; }
        public int Quality { get; set; }
        public int Amount { get; set; }
        /// <summary>
        /// if set to fale then it is not being rendered
        /// </summary>
        public bool CanUse { get; set; } = false;
        public bool CanDrop { get; set; } = true;
        public bool CanShowQuality { get; set; }

        public EquipmentItemViewModel() { }
        public EquipmentItemViewModel(int id, ProductTypeEnum productType, int quality, int amount)
            : base(productType)
        {
            ID = id;

            Quality = quality;
            Amount = amount;

            CanUse = canUse(productType);
            CanShowQuality = productType.CanShowQuality();

        }

        public EquipmentItemViewModel(HouseChestItem item)
            : this(item.ProductID, (ProductTypeEnum)item.ProductID, item.Quality, item.Quantity)
        { }

        public EquipmentItemViewModel(EquipmentItem item)
            : this(item.ID, (ProductTypeEnum)item.ProductID, item.Quality, item.Amount)
        { }


        private bool canUse(ProductTypeEnum productType)
        {
            if (productType == ProductTypeEnum.Tea
                || productType == ProductTypeEnum.MovingTicket)
                return true;

            return false;
        }


    }
}