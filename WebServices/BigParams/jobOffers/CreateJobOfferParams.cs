using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.jobOffers
{
    public class CreateJobOfferParams
    {
        public int CompanyID { get; set; }
        public int Amount { get; set; }
        public JobOfferTypeEnum OfferType { get; protected set; }
        public double MinimumSkill { get; set; }
        public int CurrencyID { get; set; }
        public decimal Cost { get; set; }
        public int? CountryID { get; set; }
    }
}
