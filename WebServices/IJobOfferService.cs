using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams.jobOffers;

namespace WebServices
{
    public interface IJobOfferService
    {
        NormalJobOffer CreateJobOffer(CreateNormalJobOfferParams parameters);
        ContractJobOffer CreateJobOffer(CreateContractJobOfferParams parameters);
        /// <summary>
        /// Takes single job (quantity--)
        /// </summary>
        /// <param name="jobOfferID"></param>
        void TakeJobOffer(int jobOfferID);
        /// <summary>
        /// remove whole job offer
        /// </summary>
        /// <param name="jobOfferID"></param>
        void TakeOutJobOffer(int jobOfferID);

        bool IsCompliantWithMinimalWage(decimal salary, int countryID);
    }
}
