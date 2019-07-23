using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using Entities.structs.jobOffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IJobOfferRepository : IRepository<JobOffer>
    {
        CountryBestJobOffers GetBestAndWorstJobOffers(int countryID, WorkTypeEnum workType);
        List<JobOffer> GetJobOffersWithoutMinimalWage(decimal minimalWage, int countryID);
    }
}
