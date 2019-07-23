using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using WebUtils;
using WebUtils.Extensions;
using Sociatis.Helpers;
using WebServices.Helpers;

namespace Sociatis.Models.Congress
{
    public class CongressVotingsViewModel : CongressBase
    {
       
        
        public string CountryName { get; set; }
        public bool IsCongressman { get; set; }
        public int CountryID { get; set; }
        public bool CanStartNewVoting { get; set; }

        public CongressVotingsViewModel(Entities.Country country) : base(country)
        {
            CountryName = country.Entity.Name;
            CountryID = country.ID;

            var citizen = SessionHelper.LoggedCitizen;

            if(citizen.ID == SessionHelper.CurrentEntity.EntityID)
            {
                if (country.Congressmen.Any(c => c.CitizenID == citizen.ID))
                {
                    IsCongressman = true;
                    CanStartNewVoting = citizen.Congressmen.First(c => c.CountryID == country.ID).LastVotingDay < GameHelper.CurrentDay;
                }
                else
                    IsCongressman = false;
            }
        }
    }
}