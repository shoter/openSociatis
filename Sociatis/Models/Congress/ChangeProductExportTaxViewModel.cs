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
    public class ChangeProductExportTaxViewModel : CongressVotingViewModel
    {
        /// <summary>
        /// In Percent(%)
        /// </summary>
        [DisplayName("New export tax")]
        [Range(0, 1000)]
        public double NewExportTax { get; set; }

        [DisplayName("Product")]
        public ProductTypeEnum ProductType { get; set; }

        public List<SelectListItem> Products { get; set; }
        [DisplayName("Foreign country")]
        public int ForeignCountryID { get; set; }

        public List<SelectListItem> Countries { get; set; } = new List<SelectListItem>();

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

        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeProductExportTax;

        public ChangeProductExportTaxViewModel()
        {
            loadProducts();
        }

        public ChangeProductExportTaxViewModel(int countryID) : base(countryID)
        {
            loadProducts();
        }

        public ChangeProductExportTaxViewModel(CongressVoting voting)
        :base(voting)
        {
            loadProducts();
            loadCountries();
        }

        public ChangeProductExportTaxViewModel(FormCollection values)
        :base(values)
        {
            if (ForeignCountryID != -1 && Persistent.Countries.Any(c => c.ID == ForeignCountryID) == false)
                throw new UserReadableException("Country does not exist!");
            if (Persistent.Countries.Any(c => c.ID == CountryID) == false)
                throw new UserReadableException("Country does not exist!");

            loadProducts();
            loadCountries();
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

            foreach (var country in Persistent.Countries.GetAll())
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
            return new ChangeProductExportTaxVotingParameters(ProductType, NewExportTax, ForeignCountryID);
        }
    }
}