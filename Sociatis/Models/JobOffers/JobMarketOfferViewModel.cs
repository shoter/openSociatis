using Common.Language;
using Entities;
using Entities.enums;
using Entities.structs.jobOffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.JobOffers
{
    public class JobMarketOfferViewModel
    {
        public int OfferID { get; set; }
        public JobOfferTypeEnum OfferType { get; set; }
        public string WorkTypeReadable { get; set; }
        public string OfferTypeReadable { get; set; }
        public string Salary { get; set; }
        public double Skill { get; set; }
        public int MinimumHP { get; set; }
        public string Length { get; set; }
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public string RegionName { get; set; }


        public JobMarketOfferViewModel(JobOfferDOM offer, Currency currency)
        {
            OfferID = offer.ID;
            OfferType = offer.JobType;
            OfferTypeReadable = offer.JobType.ToHumanReadable();
            Salary = string.Format("{0} {1}", offer.NormalSalary, currency.Symbol);
            Skill = offer.MinimumSkill;
            MinimumHP = offer.MinimumHP;
            Length = getLength(offer);
            CompanyName = offer.CompanyName;
            CompanyID = offer.CompanyID;
            WorkTypeReadable = offer.WorkType.ToHumanReadable();
            RegionName = offer.RegionName;
        }

        private string getLength(JobOfferDOM offer)
        {
            if (OfferType == JobOfferTypeEnum.Normal)
                return " - ";
            return string.Format("{0} day{1}", offer.Length, PluralHelper.S(offer.Length));
        }
    }
}