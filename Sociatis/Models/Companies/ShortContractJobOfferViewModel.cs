using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ShortContractJobOfferViewModel : ShortJobOfferViewModel
    {
        public double MinSalary { get; set; }

        public ShortContractJobOfferViewModel() { }

        public ShortContractJobOfferViewModel(ContractJobOffer contractOffer, CompanyRights companyRights)
            :base(contractOffer.JobOffer, companyRights)
        {
            MinSalary = (double)contractOffer.MinSalary;
        }
    }
}