using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICongressVotingRepository : IRepository<CongressVoting>
    {
        List<VotingStatus> GetVotingStatuses();
        List<VoteType> GetVoteTypes();
        List<VotingType> GetVotingTypes();
        List<CommentRestriction> GetCommentRestrictions();

        List<CongressVotingComment> GetVotingComments(int congressVotingID);
        List<CongressVote> GetCongressVotes(int congressVotingID);


        void AddComment(CongressVotingComment comment);
        void AddVote(CongressVote vote);

        List<CongressVoting> GetNotFinishedVotings(int countryID, VotingTypeEnum votingType);
    }
}
