using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Hospitals
{
    public class HospitalHealViewModel
    {
        public int HospitalID { get; set; }
        public double? HealingPrice { get; set; }
        public string CurrencySymbol { get; set; }
        public bool HealingEnabled { get; set; }
        public bool CanHealYou { get; set; }
        public string NoHealingReason { get; set; }
        public bool ShowHospitalName { get; set; }
        public string HospitalName { get; set; }

        public HospitalHealViewModel(Hospital hospital, IHospitalService hospitalService, IHospitalRepository hospitalRepository, bool showHospitalName = false)
        {
            HospitalID = hospital.CompanyID;

            HealingPrice = (double?)hospital.HealingPrice;

            HealingEnabled = hospital.HealingEnabled && SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen);
            if (HealingEnabled)
            {
                var citizen = SessionHelper.LoggedCitizen;
                var result = hospitalService.CanHealCitizen(SessionHelper.LoggedCitizen, hospital);
                CanHealYou = result.isSuccess;
                NoHealingReason = result.ToString("<br/>"); //if there are no errors it will be empty.
                if (CanHealYou)
                {
                    CurrencySymbol = Persistent.Countries.GetCountryCurrency(hospital.Company.Region.Country).Symbol;
                    HealingPrice = (double?)hospitalRepository.GetHealingPrice(hospital.CompanyID, citizen.ID).Cost;
                }
            }

            if (ShowHospitalName = showHospitalName)
            {
                HospitalName = hospital.Company.Entity.Name;
            }
        }
    }
}