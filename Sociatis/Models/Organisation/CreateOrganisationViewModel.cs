using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Organisation
{
    public class CreateOrganisationViewModel
    {
        [Required]
        [DisplayName("Organisation Name")]
        [StringLength(30, ErrorMessage = "Must be between 2 and 30 characters", MinimumLength = 2)]
        public string OrganisationName { get; set; }
        [DisplayName("Country")]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        [DisplayName("Fee")]
        public MoneyViewModel CountryFee { get; set; }
        [DisplayName("Fee")]
        public MoneyViewModel AdminFee { get; set; }
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
    }
}
