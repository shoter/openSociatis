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
    public class BattleGameEvent : GameEvent
    {
        public int BattleID { get; set; }
        public BattleStatusEnum BattleStatus { get; set; }
        public BattleGameEvent(BattleEvent e) : base(e.Event)
        {
            BattleStatus = (BattleStatusEnum)e.BattleStatusID;
            BattleID = e.BattleID;
        }

        public BattleGameEvent(Battle battle, BattleStatusEnum battleStatus, GameTime time)
            : base(EventTypeEnum.Battle, time.Day, time.Time)
        {
            BattleID = battle.ID;
            BattleStatus = battleStatus;
        }

        public override Event CreateEntity()
        {
            var e = base.CreateEntity();

            e.BattleEvent = new BattleEvent()
            {
                BattleID = BattleID,
                BattleStatusID = (int)BattleStatus,
            };

            return e;
        }
    }
}
