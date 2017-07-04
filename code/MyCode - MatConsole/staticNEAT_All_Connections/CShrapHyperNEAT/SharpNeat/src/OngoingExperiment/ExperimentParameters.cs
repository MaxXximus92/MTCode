using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  static StaticExperimentNS.Helper;

namespace StaticExperimentNS
{


    public class ExperimentParameters
    {
        public static String experimentName = "no name assigned";
        public static String experimentDescription = "not available";
        public static uint maxRuntime = 0;
        public static String modelSavePath = ""; // gets set during runtime
        public static String communicationPath = "";// gets set during runtime
        public static String initialNetEqParamsPath = "not given";
        public static String modelLibPathSimulate = "not given";
        public static String modelLibPathTrain = "not given";
        public static String matlabExecPath = "not given";
        public static uint numLastGenModelsToPlotAndSave = 0;
        public static uint maxGenerations = 0;
        public static uint populationSize = 0;
        public static uint cPPNInputs = 4; // for Modul approach = 2
        public static uint cPPNOutputs = 1;// for Modul approach = 8
        public static int startAngle = 0;
        public static string anglesToSimulate = "not given";
        public static int timePerAngle =0;


        static ExperimentParameters()
        {
            loadParameterFile();
        }

        public static void loadParameterFile()
        {
            try
            {

                System.IO.StreamReader input = new System.IO.StreamReader(@"expParams.txt");
                string[] line;
                while (!input.EndOfStream)
                {
                    line = input.ReadLine().Split('=');
                    //Console.WriteLine(line[0]);
                    switch (line[0].Trim())
                    {
                        case "experimentName":
                            experimentName = line[1];
                            break;
                        case "experimentDescription":
                            experimentDescription =  line[1];
                            break;
                        case "maxRuntime":
                            maxRuntime = Convert.ToUInt32(line[1]);
                            break;
                        case "initialNetEqParamsPath":
                            initialNetEqParamsPath = toFullPath(line[1]);
                            break;
                        case "modelLibPathTrain":
                            modelLibPathTrain = toFullPath(line[1]);
                            break;
                        case "modelLibPathSimulate":
                            modelLibPathSimulate = toFullPath(line[1]);
                            break;
                        case "matlabExecPath":
                            matlabExecPath = toFullPath(line[1]);
                            break;
                        case "maxGenerations":
                            maxGenerations = Convert.ToUInt32(line[1]);
                            break;
                        case "populationSize":
                            populationSize = Convert.ToUInt32(line[1]);
                            break;
                        case "numLastGenModelsToPlotAndSave":
                            numLastGenModelsToPlotAndSave = Convert.ToUInt32(line[1]);
                            break;
                        case "anglesToSimulate":
                            anglesToSimulate = line[1];
                            break;
                        case "timePerAngle":
                            timePerAngle = Convert.ToInt32(line[1]);
                            break;
                        case "startAngle":
                            startAngle = Convert.ToInt32(line[1]);
                            break;
                        default:
                            Console.WriteLine(String.Format("unknown Parameter \"{0}\" in experiment settings", line[0]));
                            break;
                    }
                }
                    
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.Message);
                System.Console.Error.WriteLine("Error reading experiment parameter file, check file location, name and formation");
                throw e;
            }
        }
    }

}
