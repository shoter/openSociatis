using Entities.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Events
{
    public class EventViewModel
    {
        public int Day { get; set; }
        public DateTime Time { get; set; }

        public EventViewModel(EventModel e)
        {
            Day = e.Day;
            Time = e.Time;
        }
    }
}
