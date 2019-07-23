using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Entities;
using WebServices;
using WebServices.Helpers;
using System.ComponentModel;
using Sociatis.Helpers;
using Entities.Extensions;

namespace Sociatis.Models.Newspapers
{
    public class CreateNewspaperViewModel
    {

        [Required]
        [StringLength(30, ErrorMessage = "Must be between 2 and 30 characters", MinimumLength = 2)]
        [DisplayName("Newspaper name")]
        public string Name { get; set; }
        public MoneyViewModel AdminFee { get; set; }
        public MoneyViewModel CountryFee { get; set; }
        public string CountryName { get; set; }

        public bool IsParty { get; set; }

        public List<MoneyViewModel> Fees
        {
            get
            {
                List<MoneyViewModel> list = new List<MoneyViewModel>();
                list.Add(AdminFee);
                list.Add(CountryFee);
                return list;
            }
        }

        public CreateNewspaperViewModel() { }

        public CreateNewspaperViewModel(Entities.Country country, ConfigurationTable configuration)
        {
            CountryName = country.Entity.Name;

            var countryMoney = Persistent.Countries.GetCountryCurrency(country);
            CountryFee = new MoneyViewModel(countryMoney, country.CountryPolicy.NewspaperCreateCost); 
            AdminFee = new MoneyViewModel(GameHelper.Gold, configuration.PartyFoundingFee);

            IsParty = SessionHelper.CurrentEntity.GetEntityType() == Entities.enums.EntityTypeEnum.Party;
        }

        public CreateNewspaperViewModel(Entities.Country country, ConfigurationTable config, string prevName) :this(country, config)
        {
            Name = prevName;
        }
    }
}