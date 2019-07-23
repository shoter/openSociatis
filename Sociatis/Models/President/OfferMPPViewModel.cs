using Sociatis.Controllers;
using Sociatis.Helpers;
using Sociatis.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.Helpers;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.President
{
    public class OfferMPPViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public Select2AjaxViewModel CountrySelect { get; set; }

        public int MinimumLength { get; set; }
        public int MaximumLength { get; set; }

        public decimal CountryGold { get; set; }
        public ImageViewModel GoldImage { get; set; }

        public OfferMPPViewModel(Entities.Country country, IMPPService mppService, IWalletService walletService)
        {
            Info = new CountryInfoViewModel(country);
            CountrySelect = Select2AjaxViewModel.Create<CountryController>(c => c.GetMPPAvailableCountries(null, 0), "OtherCountryID", null, "");
            CountrySelect.AddAdditionalData("countryID", country.ID);

            MinimumLength = ConfigurationHelper.Configuration.MinimumMPPLength;
            MaximumLength = ConfigurationHelper.Configuration.MaximumMPPLength;

            CountryGold = walletService.GetWalletMoney(country.Entity.WalletID, GameHelper.Gold.ID).Amount;
            GoldImage = Images.GetCountryCurrency(GameHelper.Gold).VM;
        }
    }
}