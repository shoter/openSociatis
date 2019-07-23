using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Helpers
{
    public static class SuffixHelper
    {
        public static string OrdinalNumber (int number)
        {
            switch(number % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }
    }
}