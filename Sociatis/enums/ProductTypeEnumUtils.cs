using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.enums
{
    public class ProductTypeEnumUtils
    {
        public static List<SelectListItem> GetFunctionsList()
        {
            var temp = new List<SelectListItem>();

            foreach (ProductTypeEnum productType in Enum.GetValues(typeof(ProductTypeEnum)).Cast<ProductTypeEnum>())
            {
                var text = "";
                switch (productType)
                {
                    case ProductTypeEnum.SellingPower:
                        text = "Sell products (shop)";
                        break;
                    case ProductTypeEnum.UpgradePoints:
                    case ProductTypeEnum.DefenseSystem:
                    case ProductTypeEnum.House:
                        continue;
                    default:
                        text = $"Produce {productType.ToHumanReadable()}";
                        break;
                }

                temp.Add(new SelectListItem()
                {
                    Text = text,
                    Value = productType.ToInt().ToString()
                });
            }

            temp = temp.OrderBy(f => f.Text).ToList();
            temp.Insert(0, new SelectListItem()
            {
                Text = "Select function",
                Value = ""
            });

            return temp;
        }
    }
}