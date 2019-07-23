using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Companies
{
    public class CreateContractJobOfferViewModel
    {
        public int CompanyID { get; set; }

        [Range(0, 100)]
        [Required]
        [DisplayName("Minimum skill")]
        public double? MinSkill { get; set; }
        [Range(1, 365)]
        [Required]
        public int Length { get; set; } = 1;
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "You must pay at least 0.01 for your employee")]
        [DisplayName("Minimum salary")]
        public double? MinimumSalary { get; set; }
        [Range(1, 100)]
        public int Amount { get; set; } = 1;
        [Range(0, 100)]
        [DisplayName("Minimum HP")]
        public int MinHP { get; set; }
        public MoneyViewModel JobMarketFee { get; set; }
        public bool PostOfferOnJobMarket { get; set; } = false;
    }
}