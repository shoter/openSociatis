using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Companies
{
    public class CreateJobOfferViewModel
    {
        public  JobOfferTypeEnum JobType { get; set; }
        public int CompanyID { get; set; }
    }
}