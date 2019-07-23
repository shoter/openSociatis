using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sociatis.Helpers;
namespace Sociatis.Models.Products
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public ImageViewModel Image { get; set; }
        public ProductTypeEnum ProductType { get; set; }

    public ProductViewModel()
        { }

        public ProductViewModel(ProductTypeEnum productType)
        {
            Image = Images.GetProductImage(productType).VM;
            Name = productType.ToString();
            ProductType = productType;
        }

    }
}
