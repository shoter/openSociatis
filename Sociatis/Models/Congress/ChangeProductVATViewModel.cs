using Common.Exceptions;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;
using WebUtils.Extensions;

namespace Sociatis.Models.Congress
{
    public class ChangeProductVATViewModel : CongressVotingViewModel
    {
        [DisplayName("New VAT")]
        [Range(0, 1000)]
        public double NewVat { get; set; }

        [DisplayName("Product")]
        public ProductTypeEnum ProductType { get; set; }

        public List<SelectListItem> Products { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeProductVAT;
        public ChangeProductVATViewModel() { }
        public ChangeProductVATViewModel(int countryID) : base(countryID) { loadProducts(); }
        public ChangeProductVATViewModel(CongressVoting voting)
        :base(voting)
        {
            loadProducts();
        }

        public ChangeProductVATViewModel(FormCollection values)
        :base(values)
        {
            loadProducts();
        }

        private void loadProducts()
        {
            Products = IEnumExtensions.ToSelectListItems<ProductTypeEnum>(p => p.ToHumanReadable(), p => p != ProductTypeEnum.SellingPower && p != ProductTypeEnum.UpgradePoints);
        }
        public override StartCongressVotingParameters CreateVotingParameters()
        {
            if (Enum.IsDefined(typeof(ProductTypeEnum), ProductType) == false)
                throw new UserReadableException("This product type is not defined");
            return new ChangeProductVatVotingParameters(ProductType, NewVat);
        }
    }
}