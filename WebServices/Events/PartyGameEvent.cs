using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;
using WebServices.Times;

namespace WebServices.Events
{
    public class PartyGameEvent : GameEvent
    {
        public int PartyID { get; set; }
        public PartyEventTypeEnum PartyEventType { get; protected set; }

        public PartyGameEvent(PartyEvent e) : base(e.Event)
        {
            PartyID = e.PartyID;
        }

        public PartyGameEvent(Party party, PartyEventTypeEnum partyEventType, GameTime gameTime)
            :base(EventTypeEnum.Party, gameTime.Day, gameTime.Time)
        {
            PartyID = party.ID;
            PartyEventType = PartyEventType;
        }

        public override Event CreateEntity()
        {
            var e = base.CreateEntity();
            e.PartyEvent = new PartyEvent()
            {
                PartyID = PartyID,
                PartyEventTypeID = (int)PartyEventType
            };
            return e;
        }


    }
}
