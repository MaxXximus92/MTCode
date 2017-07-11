using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsExperimentNS
{


    public static class Helper
    {
        public static string pathSep = Char.ToString(Path.DirectorySeparatorChar);

        public  static string toFullPath(string lokalPath) {
            return Path.GetFullPath(lokalPath);
        }

        public static double normFitness(double fitness)
        {
            return -fitness + 180;
        }
    }
}
