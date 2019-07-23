using Entities.enums;
using Entities.Repository;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebServices.Helpers;

namespace Sociatis.Models.Country
{
    public class PresidentViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public string PresidentName { get; set; }
        public ImageViewModel PresidentAvatar { get; set; }
        public int PresidentID { get; set; }

        public bool IsElectionTime { get; set; }
        public int NextElectionInDays { get; set; }

        public bool IsPlayerPresident { get; set; } = false;

        public bool CanBeCandidate { get; set; } = false;

        public PresidentViewModel(Entities.Country country, ICountryService countryService, ICountryRepository countryRepository)
        {
            Info = new CountryInfoViewModel(country);

            var president = country.President;
            if (president != null)
            {
                PresidentName = president.Entity.Name;
                PresidentAvatar = new ImageViewModel(president.Entity.ImgUrl);
                PresidentID = president.ID;
            }

            var presidentElection = countryRepository.GetLastPresidentVoting(country.ID);

            if(presidentElection.VotingStatusID == (int)VotingStatusEnum.Ongoing)
            {
                IsElectionTime = true;
            }
            else
            {
                IsElectionTime = false;
                NextElectionInDays = presidentElection.StartDay - GameHelper.CurrentDay;
            }


            var entity = SessionHelper.CurrentEntity;

            if(entity.Citizen != null)
            {
                if(entity.EntityID == PresidentID)
                {
                    IsPlayerPresident = true;
                }
                CanBeCandidate = countryService.CanCandidateAsPresident(entity.Citizen, country).isSuccess;
            }

            
        }

    }
}