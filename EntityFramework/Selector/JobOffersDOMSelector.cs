using Common.EntityFramework;
using Entities.enums;
using Entities.structs.jobOffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Selector
{
    public class JobOffersDOMSelector : IDomSelector<JobOffer, JobOfferDOM>
    {
        public IQueryable<JobOfferDOM> Select(IQueryable<JobOffer> query)
        {
            return query.Select(o => new JobOfferDOM()
            {
                ID = o.ID,
                JobType = (JobOfferTypeEnum)o.TypeID,
                NormalSalary = (double)(o.TypeID == (int)JobOfferTypeEnum.Normal ? o.NormalJobOffer.Salary : o.ContractJobOffer.MinSalary),
                MinimumSkill = (double)o.MinSkill,
                CompanyName = o.Company.Entity.Name,
                CompanyID = o.CompanyID,
                RegionName = o.Company.Region.Name,
                WorkType = (WorkTypeEnum)o.Company.WorkTypeID,
                MinimumHP = o.TypeID == (int)JobOfferTypeEnum.Contract ? o.ContractJobOffer.MinHP : o.Company.DefaultMinHP,
                //Below - contract only. For normal jobs their values will be 0.
                Length = o.TypeID == (int)JobOfferTypeEnum.Contract ? o.ContractJobOffer.Length : 0
            });
        }
    }
}
