using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;

namespace WebServices.Events
{
    public interface IGameEvent
    {
        long ID { get; }
        EventTypeEnum EventType { get; }
        int Day { get; }
        DateTime Time { get; }
        Event CreateEntity();
    }
}
