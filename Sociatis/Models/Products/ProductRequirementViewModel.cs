using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs;

namespace Sociatis.Models.Products
{
    public class ProductRequirementViewModel : ProductViewModel
    {
        public int Amount { get; set; }
        public int CurrentAmount { get; set; }

        public ProductRequirementViewModel()
        { }

        public ProductRequirementViewModel(ProductRequirement requirement, int currentAmount)
            :base(requirement.RequiredProductType)
        {
            Amount = requirement.Quantity;
            CurrentAmount = currentAmount;
        }
    }
}