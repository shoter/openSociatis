using Entities;
using Entities.enums;
using Sociatis.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Citizens
{
    public class CitizenJobShortViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        public ProductViewModel Product { get; set; }
        public CitizenJobShortViewModel(Company company)
        {
            Name = company.Entity.Name;
            Quality = company.Quality;
            Avatar = new ImageViewModel(company.Entity.ImgUrl);
            Product = new ProductViewModel((ProductTypeEnum)company.ProductID);
        }
    }
}