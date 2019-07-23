using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis_Test_Suite.Dummies
{
    public class PresidentVotingDummyCreator
    {
        private UniqueIDGenerator idGenerator = new UniqueIDGenerator();
        private PresidentVoting voting;

        public PresidentVotingDummyCreator()
        {
            voting = createVoting();
        }

        public PresidentVotingDummyCreator SetState(int votingDay, VotingStatusEnum status)
        {
            voting.StartDay = votingDay;
            voting.VotingStatusID = (int)status;

            return this;
        }

        public PresidentVoting Create(Country country)
        {
            voting.Country = country;
            voting.CountryID = country.ID;

            var _return = voting;
            voting = createVoting();
            return _return;
        }

        private PresidentVoting createVoting()
        {
            return new PresidentVoting()
            {
                ID = idGenerator.UniqueID,
                StartDay = GameHelper.CurrentDay + 7,
                VotingStatusID = (int)VotingStatusEnum.NotStarted
            };
        }
    }
}
