using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Helpers
{
    public class ScoreHelper
    {
        public static string ToString(int score)
        {
            if (score < 999)
                return score.ToString();
            if (score < 999_999)
                return string.Format("{0}k", (score / 1000).ToString());
            return string.Format("{0}m", (score / 1000_000).ToString());
        }
    }
}
