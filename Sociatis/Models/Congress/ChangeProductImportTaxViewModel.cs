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
using WebServices;
using WebServices.structs.Votings;
using WebUtils.Extensions;

namespace Sociatis.Models.Congress
{
    public class ChangeProductImportTaxViewModel : CongressVotingViewModel
    {
        [DisplayName("New import tax")]
        [Range(0, 1000)]
        public double NewImportTax { get; set; }

        [DisplayName("Product")]
        public ProductTypeEnum ProductType { get; set; }

        [DisplayName("Foreign country")]
        public int ForeignCountryID { get; set; }

        public List<SelectListItem> Countries { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Products { get; set; }

        public override int CountryID
        {
            get
            {
                return base.CountryID;
            }

            set
            {
                base.CountryID = value;
                loadCountries();
            }
        }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeProductImportTax;

        public ChangeProductImportTaxViewModel() { }
        public ChangeProductImportTaxViewModel(int countryID) : base(countryID)
        {
            loadProducts();
            loadCountries();
        }
        public ChangeProductImportTaxViewModel(CongressVoting voting)
        :base(voting)
        {
            loadProducts();
            loadCountries();
        }

        public ChangeProductImportTaxViewModel(FormCollection values)
        :base(values)
        {
            loadProducts();
            loadCountries();

            if (ForeignCountryID != -1 && Persistent.Countries.Any(c => c.ID == ForeignCountryID) == false)
                throw new UserReadableException("Country does not exist!");
            if (Persistent.Countries.Any(c => c.ID == CountryID) == false)
                throw new UserReadableException("Country does not exist!");
        }

        private void loadProducts()
        {
            Products = IEnumExtensions.ToSelectListItems<ProductTypeEnum>(p => p.ToHumanReadable(), p => p != ProductTypeEnum.SellingPower && p != ProductTypeEnum.UpgradePoints);
        }

        public void loadCountries()
        {
            Countries = new List<SelectListItem>();

            Countries.Add(new SelectListItem()
            {
                Text = "All countries",
                Value = "-1" 
            });

            foreach(var country in Persistent.Countries.GetAll())
            {
                if (CountryID == country.ID)
                    continue;
                Countries.Add(new SelectListItem()
                {
                    Text = country.Entity.Name,
                    Value = country.ID.ToString()
                });
            }
        }

        public override StartCongressVotingParameters CreateVotingParameters()
        {
            if (Enum.IsDefined(typeof(ProductTypeEnum), ProductType) == false)
                throw new UserReadableException("This product type is not defined");
            return new ChangeProductImportTaxVotingParameters(ProductType, NewImportTax, ForeignCountryID);
        }
    }
}