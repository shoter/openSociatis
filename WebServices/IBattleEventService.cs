using Entities;
using Entities.enums;
using System.Collections;
using System.Collections.Generic;

namespace WebServices
{
    public interface IBattleEventService
    {
        void AddEvent(Battle battle, BattleStatusEnum battleStatus);
    }
}