using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ShortNormalJobOfferViewModel : ShortJobOfferViewModel
    {
        public double Salary { get; set; }



        public ShortNormalJobOfferViewModel() { }

        public ShortNormalJobOfferViewModel(NormalJobOffer normalJobOffer, CompanyRights companyRights)
            :base(normalJobOffer.JobOffer, companyRights)
        {
            Salary = (double)normalJobOffer.Salary;

        }

    }
}
