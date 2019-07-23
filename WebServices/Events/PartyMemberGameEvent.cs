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
    public class PartyMemberGameEvent : PartyGameEvent
    {
        public int CitizenID { get; set; }
        public string CitizenName { get; set; }
        public int InitiatorID { get; set; }
        public string InitiatorName { get; set; }

        public MemberStatusEnum MemberStatus{ get; set; }

        public PartyMemberGameEvent(PartyMemberEvent e) : base(e.PartyEvent)
        {
            CitizenID = e.CitizenID;
            InitiatorID = e.InitiatorID;
            CitizenName = e.Citizen.Entity.Name;
            InitiatorName = e.Initiator.Name;
        }

        public PartyMemberGameEvent(Party party, Citizen citizen, Entity initiator, MemberStatusEnum memberStatus, GameTime gameTime) 
            :base(party, PartyEventTypeEnum.Member, gameTime)
        {
            CitizenID = citizen.ID;
            InitiatorID = initiator.EntityID;
            InitiatorName = initiator.Name;
            MemberStatus = memberStatus;

        }

        public override Event CreateEntity()
        {
            var e = base.CreateEntity();

            e.PartyEvent.PartyMemberEvent = new PartyMemberEvent()
            {
                CitizenID = CitizenID,
                InitiatorID = InitiatorID,
                MemberStatusID = (int)MemberStatus,
            };

            return e;
        }
    }
}
