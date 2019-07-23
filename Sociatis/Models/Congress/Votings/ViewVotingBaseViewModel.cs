using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.Helpers;

namespace Sociatis.Models.Congress.Votings
{
    public abstract class ViewVotingBaseViewModel : CongressBase
    {
        public List<CongressVotingVoterViewModel> YesVoters { get; set; } = new List<CongressVotingVoterViewModel>();
        public List<CongressVotingVoterViewModel> NoVoters { get; set; } = new List<CongressVotingVoterViewModel>();

        public int VotingID { get; set; }
        public string CreatorName { get; set; }
        public string CreatorMessage { get; set; }
        public int CreatorID { get; set; }
        public VotingStatusEnum VotingStatus { get; set; }

        public int MoreNoVotersCount { get; set; }
        public int MoreYesVotersCount { get; set; }

        public CommentRestrictionEnum CommentRestriction { get; set; }
        public bool CanSeeComments { get; set; }
        public bool CanComment { get; set; }
        public bool CanVote { get; set; }
        public string TimeLeft { get; set; }
        public bool WaitingForResolve { get; set; }

        public ViewVotingBaseViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
            :base(voting.Country)
        {
            VotingID = voting.ID;
            CreatorName = voting.Citizen.Entity.Name;
            CreatorID = voting.Citizen.ID;
            CommentRestriction = (CommentRestrictionEnum)voting.CommentRestrictionID;
            ApplyCommentRestriction(isPlayerCongressman);
            CreatorMessage = voting.CreatorMessage;

            foreach (var votes in voting.CongressVotes)
            {
                if (votes.VoteTypeID == (int)VoteTypeEnum.Yes)
                    YesVoters.Add(new CongressVotingVoterViewModel(votes.Citizen));
                else if (votes.VoteTypeID == (int)VoteTypeEnum.No)
                    NoVoters.Add(new CongressVotingVoterViewModel(votes.Citizen));
            }

            VotingStatus = (VotingStatusEnum)voting.VotingStatusID;


            MoreNoVotersCount = NoVoters.Count - 5;
            MoreYesVotersCount = YesVoters.Count - 5;

            


            var timeLeft = voting.GetTimeLeft(GameHelper.CurrentDay);

            if (timeLeft.TotalSeconds > 0)
            {
                TimeLeft = string.Format("{0:00}:{1:00}:{2:00}", (int)timeLeft.TotalHours, (int)timeLeft.Minutes, timeLeft.Seconds);
                WaitingForResolve = false;
            }
            else
            {
                TimeLeft = "00:00:00";
                WaitingForResolve = true;
            }

            CanVote = canVote && WaitingForResolve == false;

        }

        private void ApplyCommentRestriction(bool isPlayerCongressman)
        {
            if (isPlayerCongressman)
            {
                CanSeeComments = true;
                CanComment = true;
            }
            else if (CommentRestriction == CommentRestrictionEnum.CitizenCanView)
            {
                CanSeeComments = true;
            }
            else if (CommentRestriction == CommentRestrictionEnum.CitizenCanComment)
            {
                CanSeeComments = true;
                CanComment = true;
            }
        }

        public string GenerateTooltip(List<CongressVotingVoterViewModel> voters)
        {
            string tooltip = string.Empty;

            for(int i = 5; i < voters.Count; ++i)
            {
                tooltip += voters[i].Name + Environment.NewLine;
            }

            return tooltip;
        }

        public void fuckedUpTest()
        {
            var repository = DependencyResolver.Current.GetService<ICitizenRepository>();

            var citizen = repository.First();

            for (int i = 0; i < 20; ++i)
                YesVoters.Add(new CongressVotingVoterViewModel(citizen));

            for (int i = 0; i < 20; ++i)
                NoVoters.Add(new CongressVotingVoterViewModel(citizen));
        }
    }
}