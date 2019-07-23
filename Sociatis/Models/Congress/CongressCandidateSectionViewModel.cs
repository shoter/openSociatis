using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using Entities.Extensions;
using Sociatis.Helpers;
using Entities.enums;
using WebServices.Helpers;

namespace Sociatis.Models.Congress
{
    public class CongressCandidateSectionViewModel : ViewModelBase
    {
        public List<SelectListItem> CountryRegions { get; set; }
        public List<SelectListItem> CountryParties { get; set; }
        public bool CanVote { get; set; } = false;
        public int CountryID { get; set; }
        public VotingStatusEnum VotingStatus { get; set; }
        public int DaysLeft { get; set; }
        public int VotingDay { get; set; }
        public CongressInfoViewModel Info { get; set; }
        public CongressCandidateSectionViewModel(Entities.Country country)
        {
            Info = new CongressInfoViewModel(country);
            CountryRegions = CreateSelectList(country.Regions.ToList(), true, "Select Region");
            CountryRegions.Add(new SelectListItem() { Text = "All regions", Value = "ALL" });
            CountryParties = CreateSelectList(country.Parties.ToList(), p => p.Entity.Name, p => p.ID, true, "Select Party");

            var lastVoting = country.GetLastCongressCandidateVoting();

            if(SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen))
            {
                CanVote = !lastVoting.HasVoted(SessionHelper.LoggedCitizen) 
                    && lastVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing;
                
            }

            VotingStatus = (VotingStatusEnum)lastVoting.VotingStatusID;
            CountryID = country.ID;
            VotingDay = lastVoting.VotingDay;
            DaysLeft = lastVoting.VotingDay - GameHelper.CurrentDay;
        }
    }
}