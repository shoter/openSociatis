using Entities.Repository;
using Sociatis.enums;
using Sociatis.Helpers;
using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;

namespace Sociatis.Models.Country
{
    public class ConstructCompanyViewModel : ViewModelBase
    {
        public CountryInfoViewModel Info { get; set; }

        [Required]
        public string CompanyName { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public int RegionID { get; set; }

        public int CountryID { get; set; }

        public List<SelectListItem> Functions { get; set; }
        public List<SelectListItem> Regions { get; set; }

        public MoneyViewModel GoldNeeded { get; set; }
        public MoneyViewModel TreasureGold { get; set; }
        public bool CanSeeTreasury { get; set; }

        public ConstructCompanyViewModel() { }

        public ConstructCompanyViewModel(Entities.Country country, IRegionRepository regionRepository, IWalletService walletService, ICountryTreasureService countryTreasuryService)
        {
            Info = new CountryInfoViewModel(country);
            CountryID = country.ID;
            var regions = regionRepository.Where(r => r.CountryID == country.ID).
                Select(r => new
                {
                    Name = r.Name,
                    ID = r.ID
                }).ToList();

            Regions = CreateSelectList(regions, r => r.Name, r => r.ID, true);
            Functions = ProductTypeEnumUtils.GetFunctionsList();

            GoldNeeded = new MoneyViewModel(GameHelper.Gold, ConfigurationHelper.Configuration.CompanyCountryFee);
            var walletID = Persistent.Countries.GetById(country.ID).Entity.WalletID;
            TreasureGold = new MoneyViewModel(walletService.GetWalletMoney(walletID, GameHelper.Gold.ID));

            CanSeeTreasury = countryTreasuryService.CanSeeCountryTreasure(country, SessionHelper.CurrentEntity).isSuccess;
        }
    }
}