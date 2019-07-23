using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ShortJobOfferViewModel
    {
        public int Amount { get; set; }
        public int JobID { get; set; }
        public bool CanWork { get; set; }
        public string CannotWorkReason { get; set; } = "You cannot work";
        public double MinSkill { get; set; }
        public double MaxSkill { get; set; }
        public CompanyRights CompanyRights { get; set; }
        public string Skill
        {
            get
            {
                return string.Format("{0} - {1}", MinSkill, MaxSkill);
            }
        }

        public ShortJobOfferViewModel()
        {

        }

        public ShortJobOfferViewModel(JobOffer jobOffer, CompanyRights companyRights)
        {
            Amount = jobOffer.Amount;
            JobID = jobOffer.ID;
            MinSkill = (double)jobOffer.MinSkill;
            CompanyRights = companyRights;
        }
    }
}
