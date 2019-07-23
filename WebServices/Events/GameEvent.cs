using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Events
{
    public abstract class GameEvent : IGameEvent
    {
        public long ID { get; protected set; }
        public EventTypeEnum EventType { get; protected set; }
        public int Day { get; set; }
        public DateTime Time { get; set; }

        public GameEvent(Event e)
        {
            ID = e.ID;
            EventType = (EventTypeEnum)e.EventTypeID;
            Day = e.Day;
            Time = e.Time;
        }

        public GameEvent(EventTypeEnum eventType, int day, DateTime time)
        {
            EventType = eventType;
            Day = day;
            Time = time;
        }

        public virtual Event CreateEntity()
        {
            return new Event()
            {
                Day = Day,
                EventTypeID = (int)EventType,
                Time = Time
            };
        }
    }
}
