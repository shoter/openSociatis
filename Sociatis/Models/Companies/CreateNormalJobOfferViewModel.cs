using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Companies
{
    public class CreateNormalJobOfferViewModel
    {
        public int CompanyID { get; set; }
        [Range(0, 100)]
        [Required]
        public double? MinSkill { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public double? Salary { get; set; }
        [Range(1, 100)]
        public int Amount { get; set; } = 1;
        public MoneyViewModel JobMarketFee { get; set; }
        public bool PostOfferOnJobMarket { get; set; } = false;
        public CompanyInfoViewModel Info { get; set; }
    }
}
