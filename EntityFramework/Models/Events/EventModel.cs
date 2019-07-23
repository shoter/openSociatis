using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Events
{
    public class EventModel
    {
        public long EventID { get; set; }
        public int EventTypeID { get; set; }
        public int Day { get; set; }
        public DateTime Time { get; set; }

        public EventTypeEnum EventType => (EventTypeEnum)EventTypeID;
    }
}
