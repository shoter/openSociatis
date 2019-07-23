using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress
{
    public class ViewCongressViewModel : CongressBase
    {
        public string CountryName { get; set; }
        public int CongressmenCount { get; set; }
        public int NextCongressmenVoting { get; set; }
        public bool IsCountryPartyMember { get; set; } = false;
        public bool IsVotingDay { get; set; } = false;
        public bool IsInCountry { get; set; } = false;
        public bool IsCandidating { get; set; }
        public string CandidateRegion { get; set; }
        public ActualLawViewModel ActualLaw { get; set; }
        public ViewCongressViewModel(Entities.Country country)
            :base(country)
        {
            ActualLaw = new ActualLawViewModel(country.CountryPolicy);
            CongressmenCount = country.Congressmen.Count;
            CountryName = country.Entity.Name;
            NextCongressmenVoting = country.CongressCandidateVotings.Last().VotingDay;
            if (country.GetLastCongressCandidateVoting().VotingStatusID == (int)VotingStatusEnum.Ongoing)
            {
                IsVotingDay = true;
            }
            else if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen))
            {
                var citizen = SessionHelper.CurrentEntity.Citizen;
                IsCountryPartyMember = citizen.PartyMember?.Party?.CountryID == country.ID;
                IsInCountry = citizen.Region.CountryID == country.ID;
                CandidateRegion = citizen.Region.Name;
               
                if(IsCountryPartyMember)
                {
                    if (country.CongressCandidateVotings.Last().CongressCandidates.Any(c => c.CandidateID == citizen.ID))
                        IsCandidating = true;
                }
            }
        }
    }
}