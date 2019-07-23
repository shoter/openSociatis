using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IPresidentVotingRepository : IRepository<PresidentVoting>
    {
        IQueryable<PresidentVoting> OngoingVotings { get; }
        IQueryable<PresidentVoting> NotStartedVotings { get; }
        IQueryable<PresidentVoting> NotFinishedVotings { get; }


        PresidentCandidate GetCandidateByID(int ID);
        void AddCandidate(PresidentCandidate candidate);
        void AddVote(PresidentVote vote);

        List<PresidentVoting> GetInactiveNotLastPresidentVotings(int currentDay);
        /// <summary>
        /// Returns list of candidates which have state that should not be possible.
        /// </summary>
        List<PresidentCandidate> GetCandidatesInIncorrectState();
    }
}
