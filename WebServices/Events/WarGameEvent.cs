using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using WebServices.Times;

namespace WebServices.Events
{
    public class WarGameEvent : GameEvent
    {
        public int WarID { get; set; }
        public WarStatusEnum WarStatus { get; set; }
        public WarGameEvent(WarEvent e) : base(e.Event)
        {
            WarStatus = (WarStatusEnum)e.WarStatusID;
            WarID = e.WarID;
        }

        public WarGameEvent(War war, WarStatusEnum warStatus, GameTime time)
            : base(EventTypeEnum.War, time.Day, time.Time)
        {
            WarID = war.ID;
            WarStatus = warStatus;
        }

        public override Event CreateEntity()
        {
            var e = base.CreateEntity();
            e.WarEvent = new WarEvent()
            {
                WarID = WarID,
                WarStatusID = (int)WarStatus
            };

            return e;
        }
    }
}
