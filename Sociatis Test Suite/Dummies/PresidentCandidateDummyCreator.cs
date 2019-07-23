using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class PresidentCandidateDummyCreator
    {
        private PresidentCandidate presidentCandidate;
        private CitizenDummyCreator citizenCreator = new CitizenDummyCreator();
        private PresidentVoteDummyGenerator voteGenerator = new PresidentVoteDummyGenerator();

        private static UniqueIDGenerator idGenerator = new UniqueIDGenerator();
        private int votesNumber = 0;

        public PresidentCandidateDummyCreator()
        {
            presidentCandidate = createCandidate();
        }

        public PresidentCandidate Create(PresidentVoting voting)
        {
            presidentCandidate.PresidentVoting = voting;
            presidentCandidate.VotingID = voting.ID;

            for(int i = 0;i < votesNumber; ++i)
            {
                voteGenerator.Create(presidentCandidate);
            }


            var _return = presidentCandidate;
            presidentCandidate = createCandidate();
            return _return;
        }

        public void setVotesNumber(int number)
        {
            votesNumber = number;
        }

        private PresidentCandidate createCandidate()
        {
            var citizen = citizenCreator.Create();

            return new PresidentCandidate()
            {
                CandidateID = citizen.ID,
                ID = idGenerator.UniqueID,
                CandidateStatusID = (int)PresidentCandidateStatusEnum.WaitingForElectionEnd,
                Citizen = citizen
            };
        }

    }
}
