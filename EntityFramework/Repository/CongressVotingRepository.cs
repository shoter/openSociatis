using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Entities.enums;
using Common.EntityFramework;

namespace Entities.Repository
{
    public class CongressVotingRepository : RepositoryBase<CongressVoting, SociatisEntities>, ICongressVotingRepository
    {
        public CongressVotingRepository(SociatisEntities context) : base(context)
        {
        }

        public List<CommentRestriction> GetCommentRestrictions()
        {
            return context.CommentRestrictions.ToList();
        }

        public List<CongressVote> GetCongressVotes(int congressVotingID)
        {
            return context.CongressVotes
                .Where(vote => vote.CongressVotingID == congressVotingID)
                .ToList();
        }

        public List<VoteType> GetVoteTypes()
        {
            return context.VoteTypes.ToList();
        }

        public List<CongressVotingComment> GetVotingComments(int congressVotingID)
        {
            return context.CongressVotingComments
                .Where(vote => vote.CongressVotingID == congressVotingID)
                .ToList();
        }

        public List<VotingStatus> GetVotingStatuses()
        {
            return context.VotingStatuses.ToList();
        }

        public List<VotingType> GetVotingTypes()
        {
            List<int> allowedVotes = new List<int>();

            foreach (VotingTypeEnum votingType in Enum.GetValues(typeof(VotingTypeEnum)))
                allowedVotes.Add((int)votingType);

            return context.VotingTypes
                .Where(v => allowedVotes.Contains(v.ID))
                .Where(v => v.AlwaysVotable)
                .ToList();
        }

        public void AddComment(CongressVotingComment comment)
        {
            context.CongressVotingComments.Add(comment);
        }

        public void AddVote(CongressVote vote)
        {
            context.CongressVotes.Add(vote);
        }

        public List<CongressVoting> GetNotFinishedVotings(int countryID, VotingTypeEnum votingType)
        {
            return Where(v => v.CountryID == countryID && v.VotingTypeID == (int)votingType
            && (v.VotingStatusID == (int)VotingStatusEnum.NotStarted || v.VotingStatusID == (int)VotingStatusEnum.Ongoing))
            .ToList();
        }

        

    }
}
