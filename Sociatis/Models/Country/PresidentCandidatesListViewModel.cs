using Common.Operations;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class PresidentCandidatesListViewModel
    {
        public List<PresidentCandidateViewModel> Candidates { get; set; } = new List<PresidentCandidateViewModel>();
        public CountryInfoViewModel Info { get; set; }
        public int ElectionDay { get; set; }
        public bool CanCandidate { get; set; }
        public string CannotCandidateReason { get; set; }

        public bool IsElectionDay { get; set; }
        public bool CanVote { get; set; } = false;

        public PresidentCandidatesListViewModel(Entities.PresidentVoting voting, MethodResult canCandidateResult, bool canVote)
        {
            foreach(var candidate in voting.PresidentCandidates.ToList())
            {
                Candidates.Add(new PresidentCandidateViewModel(candidate));
            }

            Info = new CountryInfoViewModel(voting.Country);

            ElectionDay = voting.StartDay;
            CanCandidate = canCandidateResult.isSuccess;
            if (CanCandidate == false)
                CannotCandidateReason = canCandidateResult.Errors[0].ToString();

            IsElectionDay = voting.VotingStatusID == (int)VotingStatusEnum.Ongoing;
            CanVote = canVote;
        }
    }
}