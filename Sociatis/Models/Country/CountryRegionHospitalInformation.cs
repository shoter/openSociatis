using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class CountryRegionHospitalInformation
    {
        public SmallEntityAvatarViewModel Avatar { get; set; }
        public int HospitalID { get; set; }
        public string HospitalName { get; set; }
        public int Quality { get; set; }

        public CountryRegionHospitalInformation(Hospital hospital)
        {
            Quality = hospital.Company.Quality;
            var entity = hospital.Company.Entity;

            HospitalID = hospital.CompanyID;
            HospitalName = entity.Name;

            Avatar = new SmallEntityAvatarViewModel(entity);
            Avatar.DisableNameInclude();
        }
    }
}