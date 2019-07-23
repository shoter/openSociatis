using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.jobOffers
{
    public class CreateContractJobOfferParams : CreateJobOfferParams
    {
        public CreateContractJobOfferParams()
        {
            base.OfferType = JobOfferTypeEnum.Contract;
        }

        public double MinimumSalary { get; set; }
        public int MinimumHP { get; set; }
        public int Length { get; set; }
        public int SigneeID { get; set; }
    }
}
