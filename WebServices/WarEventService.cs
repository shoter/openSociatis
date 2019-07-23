using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Events;
using WebServices.Times;

namespace WebServices
{
    public class WarEventService : BaseService, IWarEventService
    {
        private readonly IEventRepository eventRepository;

        public WarEventService(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }

        public void AddEvent(War war, WarStatusEnum warStatus)
        {
            Debug.Assert(war.ID > 0);
            var e = new WarGameEvent(war, warStatus, GameTime.Now);

            eventRepository.Add(e.CreateEntity());
            ConditionalSaveChanges(eventRepository);
        }
    }
}
