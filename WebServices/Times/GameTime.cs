using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace WebServices.Times
{
    public class GameTime
    {
        public int Day { get; set; }
        public DateTime Time { get; set; }

        public static GameTime Now => new GameTime(GameHelper.CurrentDay, DateTime.Now);

        public GameTime(int day, DateTime time)
        {
            Day = day;
            Time = time;
        }

    }
}
