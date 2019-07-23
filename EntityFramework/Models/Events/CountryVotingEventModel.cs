using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Events
{
    public class CountryVotingEventModel : CountryEventModel
    {
        public int VotingID { get; set; }
        public int VotingStatusID { get; set; }

        public VotingStatusEnum VotingStatus => (VotingStatusEnum)VotingStatusID;
    }
}
