using Common;
using Entities;
using Entities.enums;
using Entities.Extensions;
using SociatisCommon.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Products
{
    public class ProductStockViewModel : ProductViewModel
    {
        public int Amount { get; set; }
        public int Quality { get; set; }
        public bool ShowQuality { get; set; } = true;
        public bool ShowQuantity { get; set; } = true;

        public ProductStockViewModel()
        { }

        public ProductStockViewModel(Company company, EquipmentItem stock) : base((ProductTypeEnum)stock.ProductID)
        {
            Amount = stock.Amount;
            Quality = stock.Quality;
            if (((ProductTypeEnum)stock.ProductID).GetEnumAttribute<QualityProductAttribute>() == null)
                ShowQuality = false;
        }
    }
}