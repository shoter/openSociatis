using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class JobOfferExtensions
    {
        public static JobOfferTypeEnum GetJobOfferType(this JobOffer jobOffer)
        {
            return (JobOfferTypeEnum)jobOffer.TypeID;
        }

        public static double GetStartingSalary(this JobOffer jobOffer)
        {
            switch(jobOffer.GetJobOfferType())
            {
                case JobOfferTypeEnum.Normal:
                    return (double)jobOffer.NormalJobOffer.Salary;
                case JobOfferTypeEnum.Contract:
                    return (double)jobOffer.ContractJobOffer.MinSalary;
                case JobOfferTypeEnum.Both:
                    throw new Exception("I do not know how to handle that XD");
            }

            throw new Exception("GetStartingSalary Exception");
        }
    }
}
