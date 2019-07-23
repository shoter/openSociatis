using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Times;

namespace WebServices.Events
{
    public class CountryVotingGameEvent : CountryGameEvent
    {
        public VotingStatusEnum CongressVotingStatus { get; set; }
        public int VotingID { get; set; }

        public CountryVotingGameEvent(CountryVotingEvent e) : base(e.CountryEvent)
        {
            CongressVotingStatus = (VotingStatusEnum)e.VotingStatusID;
        }

        public CountryVotingGameEvent(Event e) : this(e.CountryEvent.CountryVotingEvent) { }

        public CountryVotingGameEvent(CongressVoting voting, VotingStatusEnum votingStatus, GameTime time)
            : base(voting.Country, CountryEventTypeEnum.Voting, time)
        {
            VotingID = voting.ID;
            CongressVotingStatus = votingStatus;
        }


        public override Event CreateEntity()
        {
            var e = base.CreateEntity();

            e.CountryEvent.CountryVotingEvent = new CountryVotingEvent()
            {
                VotingID = VotingID,
                VotingStatusID = (int)CongressVotingStatus
            };

            return e;
        }

    }
}
