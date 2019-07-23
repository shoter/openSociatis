using Entities;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServices.Helpers
{
    public class GameHelper
    {
        private static Mutex mutex = new Mutex();
        private static int? _currentDay = null;
        private static Currency _gold;

        public static int CurrentDay
        {
            get
            {
                if (_currentDay == null)
                {
                    throw new Exception("You must run init");
                }

                return _currentDay.Value;
            }
            set
            {
                lock (mutex)
                {
                    _currentDay = value;
                }
            }
        }

        public static Currency Gold
        {
            get
            {
                if (_gold == null)
                {
                    throw new Exception("You must run init");
                }

                return _gold;
            }
            set
            {
                lock (mutex)
                {
                    _gold = value;
                }
            }
        }
        private static int? activePlayers = null;
        public static int ActivePlayers
        {
            get
            {
                if (activePlayers.HasValue == false)
                    return -1;
                return activePlayers.Value;
            }
            set
            {
                activePlayers = value;
            }
        }
        public static bool IsDayChange { get; set; } = false;

        public static bool Initialized { get; private set; } = false;

        public static DateTime? LastDayChangeTime { set; get; } = null;
        public static DateTime? LastDayChangeRealTime { get; set; } = null;

        public static void Init(IConfigurationRepository configurationRepository, ICurrencyRepository currencyRepository, ICitizenService citizenService)
        {
            CurrentDay = configurationRepository.GetCurrentDay();
            Gold = currencyRepository.Gold;
            LastDayChangeTime = configurationRepository.GetLastDayChangeTime();
            ActivePlayers = citizenService.GetActivePlayerCount();
            Initialized = true;
        }
    }
}
