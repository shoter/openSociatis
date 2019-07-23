using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.enums;
using Entities.Models.Events;

namespace Sociatis.Models.Events
{
    public class CountryVotingEventViewModel : CountryEventViewModel
    {
        public int VotingID { get; set; }
        public VotingStatusEnum VotingStatus { get; set; }

        public string Message { get; set; }
        public CountryVotingEventViewModel(CountryVotingEventModel e) : base(e)
        {
            VotingID = e.VotingID;
            VotingStatus = e.VotingStatus;

            switch (VotingStatus)
            {
                case VotingStatusEnum.Accepted:
                    Message = "has been accepted";
                    break;
                case VotingStatusEnum.Rejected:
                    Message = "has been rejected";
                    break;
                case VotingStatusEnum.Started:
                    Message = "has started";
                    break;
            }
        }
    }
}
