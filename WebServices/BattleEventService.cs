using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using Entities.Repository;
using WebServices.Events;
using WebServices.Times;

namespace WebServices
{
    public class BattleEventService : BaseService, IBattleEventService
    {
        private readonly IEventRepository eventRepository;
        public BattleEventService(IEventRepository eventRepository)
        {
            this.eventRepository = eventRepository;
        }
        public void AddEvent(Battle battle, BattleStatusEnum battleStatus)
        {
            Debug.Assert(battle.ID > 0);
            var e = new BattleGameEvent(battle,  battleStatus, GameTime.Now);

            eventRepository.Add(e.CreateEntity());
            ConditionalSaveChanges(eventRepository);
        }
    }
}
