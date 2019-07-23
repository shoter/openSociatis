using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class CreatePartyViewModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "Must be between 2 and 30 characters", MinimumLength = 2)]
        public string Name { get; set; }
        public MoneyViewModel AdminFee { get; set; }
        public MoneyViewModel CountryFee { get; set; }

        public CreatePartyViewModel() { }
        public CreatePartyViewModel(MoneyViewModel adminFee, MoneyViewModel countryFee)
        {
            this.AdminFee = adminFee;
            this.CountryFee = countryFee;
        }
    }
}