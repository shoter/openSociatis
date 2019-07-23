using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.structs.jobOffers;
using Entities.enums;
using Common.EntityFramework;

namespace Entities.Repository
{
    public class JobOfferRepository : RepositoryBase<JobOffer, SociatisEntities>, IJobOfferRepository
    {
        public JobOfferRepository(SociatisEntities context) : base(context)
        {
        }

        public CountryBestJobOffers GetBestAndWorstJobOffers(int countryID, WorkTypeEnum workType)
        {
            var query = Where(offer => offer.CountryID == countryID);
            if (workType != WorkTypeEnum.Any)
                query = query.Where(offer => offer.Company.WorkTypeID == (int)workType);
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            //Why am i suppressing warning? Cause it's false! I need to check for null because normalJobOffer may be null sometimes! I cannot do it with ?. because
            //query providers do not support those yet.
            var salary = query
                .Select(offer => new { Salary = (offer.NormalJobOffer.Salary == null ? (decimal)0.0 : offer.NormalJobOffer.Salary)
                + (offer.ContractJobOffer.MinSalary == null ? (decimal)0.0 : offer.ContractJobOffer.MinSalary) });

            var minSal = salary
                .OrderBy(x => x.Salary);

            var maxSal = salary
                .OrderByDescending(x => x.Salary);

            return (from o in context.JobOffers
                   select new CountryBestJobOffers()
                   {
                       MinimumSalary = minSal.FirstOrDefault().Salary,
                       MaximumSalary = maxSal.FirstOrDefault().Salary,

                       MinimumSkill = query.OrderBy(offer => offer.MinSkill).FirstOrDefault().MinSkill,
                       MaximumSkill = query.OrderByDescending(offer => offer.MinSkill).FirstOrDefault().MinSkill
                   }).FirstOrDefault();
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'    */
        }


        public override void Remove(JobOffer entity)
        {
            if (entity.ContractJobOffer != null)
                context.ContractJobOffers.Remove(entity.ContractJobOffer);
            if (entity.NormalJobOffer != null)
                context.NormalJobOffers.Remove(entity.NormalJobOffer);

            base.Remove(entity);
        }

        public List<JobOffer> GetJobOffersWithoutMinimalWage(decimal minimalWage, int countryID)
        {
            return Where(offer => offer.NormalJobOffer.Salary < minimalWage || offer.ContractJobOffer.MinSalary < minimalWage)
                .Where(offer => offer.Company.Region.CountryID == countryID)
                .ToList();
        }
    }
}
