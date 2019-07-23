using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class PresidentVoteDummyGenerator
    {
        private static UniqueIDGenerator idGenerator = new UniqueIDGenerator();
        private CitizenDummyCreator citizenGenerator = new CitizenDummyCreator();
        private PresidentVote presidentVote;

        public PresidentVoteDummyGenerator()
        {
            presidentVote = createVote();
        }

        public PresidentVote Create(PresidentCandidate candidate)
        {
            presidentVote.PresidentCandidate = candidate;
            presidentVote.PresidentVoting = candidate.PresidentVoting;
            presidentVote.PresidentVotingID = candidate.VotingID;
            presidentVote.CandidateID = candidate.ID;
            candidate.PresidentVotes.Add(presidentVote);
            candidate.PresidentVoting.PresidentVotes.Add(presidentVote);

            var _return = presidentVote;
            presidentVote = createVote();
            return _return;
        }

        public PresidentVoteDummyGenerator SetVotingCitizen(Citizen citizen)
        {
            presidentVote.CitizenID = citizen.ID;
            presidentVote.Citizen = citizen;
            citizen.PresidentVotes.Add(presidentVote);

            return this;
        }

        private PresidentVote createVote()
        {
            var citizen = citizenGenerator.Create();

            var vote = new PresidentVote()
            {
                ID = idGenerator.UniqueID,
                Citizen = citizen,
                CitizenID = citizen.ID
            };

            citizen.PresidentVotes.Add(vote);

            return vote;
        }
    }
}
