using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class PresidentVotingRepository : RepositoryBase<PresidentVoting, SociatisEntities>, IPresidentVotingRepository
    {
        public IQueryable<PresidentVoting> OngoingVotings => Where(v => v.VotingStatusID == (int)VotingStatusEnum.Ongoing);
        public IQueryable<PresidentVoting> NotStartedVotings => Where(v => v.VotingStatusID == (int)VotingStatusEnum.NotStarted);
        public IQueryable<PresidentVoting> NotFinishedVotings => Where(v => v.VotingStatusID == (int)VotingStatusEnum.NotStarted ||v.VotingStatusID == (int)VotingStatusEnum.Ongoing);

        public PresidentVotingRepository(SociatisEntities context) : base(context)
        {
        }

        public PresidentCandidate GetCandidateByID(int ID)
        {
            return context.PresidentCandidates.First(c => c.ID == ID);
        }

        public void AddCandidate(PresidentCandidate candidate)
        {
            context.PresidentCandidates.Add(candidate);
        }

        public void AddVote(PresidentVote vote)
        {
            context.PresidentVotes.Add(vote);
        }

        public List<PresidentVoting> GetInactiveNotLastPresidentVotings(int currentDay)
        {
            return Where(vot => (vot.VotingStatusID == (int)VotingStatusEnum.NotStarted || vot.VotingStatusID == (int)VotingStatusEnum.Ongoing)
            && vot.StartDay < currentDay).ToList();
        }

        public List<PresidentCandidate> GetCandidatesInIncorrectState()
        {
            return Where(vot => vot.VotingStatusID == (int)VotingStatusEnum.Finished)
                .SelectMany(vot => vot.PresidentCandidates)
                .Where(cand => cand.CandidateStatusID == (int)PresidentCandidateStatusEnum.WaitingForElectionEnd)
                .ToList();
        }
    }
}
