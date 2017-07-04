using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OngoingModel;
using System.Diagnostics;

namespace ModelTest
{
    class ModelTestClass
    {
        static void Main(string[] args)
        {
            Spikenet m = new Spikenet(256,30,"settings.xls","");
            Console.WriteLine("sdf");
            float[,] a = m.WeightsMatrix;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = 2;
                }
            }
            m.WeightsMatrix = a;
            float[,] startsettings = new float[1, 2] { { 0.65f, 35 } };//, { 0.65f, 75 }, { 0.65f, 105 } };
            Stopwatch sw = new Stopwatch();
            sw.Start();
            m.simulate(120000, startsettings, 1);
            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }
    }
}
