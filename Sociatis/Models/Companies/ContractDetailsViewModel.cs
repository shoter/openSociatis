using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Companies
{
    public class ContractDetailsViewModel
    {
        public int JobOfferID { get; set; }
        public double MinSkill { get; set; }
        public double MaxSkill { get; set; }
        public string Skill { get { return string.Format("{0} - {1}", MinSkill, MaxSkill); } }

        public double MinSalary { get; set; }
        public int MinHP { get; set; }
        public int Length { get; set; }

        public ContractDetailsViewModel() { }

        public ContractDetailsViewModel(ContractJobOffer contract, int jobOfferID)
        {
            MinSkill = (double)contract.JobOffer.MinSkill;
            MinSalary = (double)contract.MinSalary;
            MinHP = contract.MinHP;
            Length = contract.Length;

            JobOfferID = jobOfferID;
        }


    }
}
