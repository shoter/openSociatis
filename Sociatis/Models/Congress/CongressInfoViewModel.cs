using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.Extensions;
using WebServices.Helpers;
using Sociatis.Helpers;

namespace Sociatis.Models.Congress
{
    public class CongressInfoViewModel
    {
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public int MemberCount { get; set; }
        public int CadencyDaysLeft { get; set; }
        public ImageViewModel CongressImageViewModel { get; set; }
        public ImageViewModel CountryFlagImageViewModel { get; set; }

        public CongressInfoViewModel(Entities.Country country)
        {
            CountryName = country.Entity.Name;
            CountryID = country.ID;
            MemberCount = country.Congressmen.Count;
            CongressImageViewModel = Images.GetCountryFlag(country).VM;
            CadencyDaysLeft = country.GetLastCongressCandidateVoting().VotingDay - GameHelper.CurrentDay;
        }


    }
}