using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class CompanyJobOffersView
    {
        public List<ShortJobOfferViewModel> JobOffers { get; set; } = new List<ShortJobOfferViewModel>();
        public CompanyInfoViewModel Info { get; set; }
        public int CompanyID { get; set; }

        public CompanyJobOffersView(Company company)
        {
            Info = new CompanyInfoViewModel(company);

            CompanyID = company.ID;

            foreach (var jobOffer in company.JobOffers)
            {
                var shortJobOfferVM = createShortJobOffer(jobOffer, (WorkTypeEnum)company.WorkTypeID, Info.CompanyRights);
                JobOffers.Add(shortJobOfferVM);
            }
        }

        private ShortJobOfferViewModel createShortJobOffer(JobOffer jobOffer, WorkTypeEnum workType, CompanyRights companyRights)
        {
            ShortJobOfferViewModel vm;
            switch ((JobTypeEnum)jobOffer.TypeID)
            {
                case JobTypeEnum.Normal:
                    vm = new ShortNormalJobOfferViewModel(jobOffer.NormalJobOffer, companyRights);
                    break;
                case JobTypeEnum.Contracted:
                    vm = new ShortContractJobOfferViewModel(jobOffer.ContractJobOffer, companyRights);
                    break;
                default:
                    throw new ArgumentException();
            }

            vm.CanWork = true;
            if (SessionHelper.CurrentEntity.GetEntityType() == EntityTypeEnum.Citizen)
            {
                switch (workType)
                {
                    case WorkTypeEnum.Construction:
                        vm.CanWork = SessionHelper.CurrentEntity.Citizen.Construction >= jobOffer.MinSkill;
                        if (!vm.CanWork)
                            vm.CannotWorkReason = "Your skill is not good enough";
                        break;
                    case WorkTypeEnum.Manufacturing:
                        vm.CanWork = SessionHelper.CurrentEntity.Citizen.Manufacturing >= jobOffer.MinSkill;
                        if (!vm.CanWork)
                            vm.CannotWorkReason = "Your skill is not good enough";
                        break;
                    case WorkTypeEnum.Raw:
                        vm.CanWork = SessionHelper.CurrentEntity.Citizen.Raw >= jobOffer.MinSkill;
                        if (!vm.CanWork)
                            vm.CannotWorkReason = "Your skill is not good enough";
                        break;
                }
            }
            else
            {
                vm.CanWork = false;
                vm.CannotWorkReason = "Only citizens can work";
            }

            vm.Amount = jobOffer.Amount;
            vm.JobID = jobOffer.ID;
            return vm;
        }
    }
}