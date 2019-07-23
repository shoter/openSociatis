using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ICountryEventService
    {
        void AddVotingEvent(CongressVoting voting, VotingStatusEnum votingStatus);
    }
}
