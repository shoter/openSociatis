using Entities;
using Entities.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Test
{
    class Program
    {
        public static double  newStrength(double str)
        {
            double A = 5.117435297;
            double B = 1.49897E-05;
            double C = 19.41608989;
            double D = 108098.9343;
            double E = 4.68E+00;    
            double F = 1.070000161;
            double a = 1.12451E-05;
            double b = 0.000250554;
            double c = 9.34872E-06;

            double gain  = D / Math.Pow((Math.Pow(str, F) + a * Math.Pow(str, F) + b * Math.Pow(str, A) + c * Math.Pow(str, B) + C), E);
            gain = Math.Round(gain, 4);
            return str + gain;
        }

        static void Main(string[] args)
        {
           // while (true)
           // {
                double str = 0.0;
                Console.Clear();
                File.WriteAllText("log.txt", "");
                using (var writer = File.AppendText("log.txt"))
                    for (int i = 0; i < 20000; ++i)
                    {
                        var msg = string.Format("{0} {1}", i, str);

                        writer.WriteLine(msg);
                        str = newStrength(str);

                    }
                Console.WriteLine("done");
//
            

        }
    }
}
