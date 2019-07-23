using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities.Extensions;
using Entities.enums;
using Sociatis.Helpers;
using WebServices;
using System.Web.Mvc;
using Entities.structs.Newspapers;
using WebServices.Helpers;
using Entities.Models.Events;
using Sociatis.Models.Events;

namespace Sociatis.Models.Home
{
    public class HomeIndexViewModel
    {
        public bool IsCongressVotingDay { get; set; } = false;
        public int CountryID { get; set; }
        public bool CanVote { get; set; } = false;

        public DateTime? LastDayChange { get; set; }


        public bool IsPresidentElectionDay { get; set; }
        public bool IsPartyPresidentElectionDay { get; set; }
        public int CitizenPartyID { get; set; }
        public bool CanVoteInPresidentElections { get; set; } = false;
        public bool CanVoteInPartyPresidentElections { get; set; } = false;

        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Quality { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Currencies { get; set; } = new List<SelectListItem>();

        public List<HomeActiveBattleViewModel> ActiveBattles { get; set; } = new List<HomeActiveBattleViewModel>();

        public List<HomeArticleSpotlightViewModel> PopularArticles { get; set; } = new List<HomeArticleSpotlightViewModel>();
        public List<HomeArticleSpotlightViewModel> NewArticles { get; set; } = new List<HomeArticleSpotlightViewModel>();
        public List<HomeArticleSpotlightViewModel> AdminArticles { get; set; } = new List<HomeArticleSpotlightViewModel>();

        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();


        public bool DisplayDebug { get; set; } = false;

        public HomeIndexViewModel(Entities.Country country, List<Entities.Battle> activeBattles, 
            List<EventModel> events,
            ICongressCandidateService congressCandidateService, List<ArticleDom> newestArticles, List<ArticleDom> popularArticles, List<ArticleDom> adminArticles)
        {
            var lastVoting = country.GetLastCongressCandidateVoting();
            CountryID = country.ID;
            LastDayChange = GameHelper.LastDayChangeRealTime;

            foreach (var e in events)
                Events.Add(new CountryVotingEventViewModel(e as CountryVotingEventModel));

            if (lastVoting.Is(VotingStatusEnum.Ongoing))
            {
                IsCongressVotingDay = true;
                if (SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen))
                    CanVote = congressCandidateService.CanVote(SessionHelper.LoggedCitizen, lastVoting);
            }

            var presidentVoting = country.PresidentVotings.Last();

            if (presidentVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
            {
                IsPresidentElectionDay = true;
                var entity = SessionHelper.CurrentEntity;

                if (entity.Citizen != null)
                {
                    var citizen = entity.Citizen;
                    if (presidentVoting.PresidentVotes.Any(v => v.CitizenID == citizen.ID) == false)
                        CanVoteInPresidentElections = true;
                }
            }

            if (SessionHelper.CurrentEntity.GetEntityType() == EntityTypeEnum.Citizen)
            {
                var citizen = SessionHelper.LoggedCitizen;
                var partyPresidentVoting = citizen.PartyMember?.Party?.PartyPresidentVotings?.Last();

                if (partyPresidentVoting != null && partyPresidentVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
                {
                    IsPartyPresidentElectionDay = true;

                    if (partyPresidentVoting.PartyPresidentVotes.Any(v => v.CitizenID == citizen.ID) == false)
                        CanVoteInPartyPresidentElections = true;

                    CitizenPartyID = partyPresidentVoting.PartyID;
                }

            }

            foreach (var battle in activeBattles)
                ActiveBattles.Add(new HomeActiveBattleViewModel(battle));

            addProductsAndQualities();
            addCurrencies();

            foreach (var newArt in newestArticles)
            {
                NewArticles.Add(new HomeArticleSpotlightViewModel(newArt));
            }

            foreach (var popularArt in popularArticles)
            {
                PopularArticles.Add(new HomeArticleSpotlightViewModel(popularArt));
            }

            foreach (var adminArt in adminArticles)
            {
                AdminArticles.Add(new HomeArticleSpotlightViewModel(adminArt));
            }


            DisplayDebug = SessionHelper.LoggedCitizen.GetPlayerType() >= PlayerTypeEnum.Admin;
        }

        private void addCurrencies()
        {
            foreach (CurrencyTypeEnum currency in Enum.GetValues(typeof(CurrencyTypeEnum)))
            {
                Currencies.Add(new SelectListItem()
                {
                    Text = currency.ToString(),
                    Value = ((int)currency).ToString()
                });
            }
        }

        private void addProductsAndQualities()
        {
            foreach (ProductTypeEnum product in Enum.GetValues(typeof(ProductTypeEnum)))
            {
                Products.Add(new SelectListItem()
                {
                    Text = product.ToString(),
                    Value = ((int)product).ToString()
                });
            }

            for (int i = 0; i < 5; ++i)
            {
                Quality.Add(new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }
        }
    }
}