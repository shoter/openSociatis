using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Votings
{
    public abstract class StartCongressVotingParameters
    {
        public Citizen Creator { get; set; }
        public Country Country { get; set; }
        public abstract VotingTypeEnum VotingType { get; protected set; }
        public string CreatorMessage { get; set; }
        public CommentRestrictionEnum CommentRestriction { get; set; }
        public int VotingLength { get; set; }

        public abstract void FillCongressVotingArguments(CongressVoting voting);
    }
}
