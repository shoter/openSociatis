using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.jobOffers
{
    public class CreateNormalJobOfferParams : CreateJobOfferParams
    {
        public CreateNormalJobOfferParams()
        {
            base.OfferType = JobOfferTypeEnum.Normal;
        }

        public double Salary { get; set; }
    }
}
