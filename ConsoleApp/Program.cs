using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.enums;
using Common.Extensions;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (MilitaryRankEnum rank in Enum.GetValues(typeof(MilitaryRankEnum)))
            {
                double bonus =  1.0 + (double)rank * 0.02;
                int exp = MilitaryRankEnumExtensions.ExperienceNeededForRank[rank];

                Console.WriteLine("|-");
                Console.WriteLine($"|{rank.ToHumanReadable().FirstUpper()}");
                Console.WriteLine($"|{exp}");
                Console.WriteLine($"|{bonus}");
                Console.WriteLine("");
            }
            Console.ReadKey();
        }
    }
}
