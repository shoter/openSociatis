using Common.Extensions;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Gifts
{
    public class ProductGiftViewModel
    {
        public ImageViewModel ProductImage { get; set; }
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public int Amount { get; set; }
        public int Quality { get; set; }

        public ProductGiftViewModel(EquipmentItem item)
        {
            ProductID = item.ProductID;
            ProductImage = Images.GetProductImage(EquipmentItemExtension.GetProductType(item)).VM;
            ProductName = item.GetProductType().ToHumanReadable().FirstUpper();
            Amount = item.Amount;
            Quality = item.Quality;
        }
    }
}