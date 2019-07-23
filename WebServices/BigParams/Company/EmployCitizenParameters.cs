using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.Company
{
    public class EmployCitizenParameters
    {
        public int CitizenID { get; set; }
        public int CompanyID { get; set; }
        public int JobOfferID { get; set; }
        public ContractJobOffer ContractOffer { get; set; }
        public double Salary { get; set; }
    }
}
