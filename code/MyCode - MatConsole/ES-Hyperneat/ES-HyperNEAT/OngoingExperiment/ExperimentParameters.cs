using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  static EsExperimentNS.Helper;

namespace EsExperimentNS
{


    public class ExperimentParameters
    {
        public static String experimentName = "no name assigned";
        public static String experimentDescription = "not available";
        public static uint training_maxTrainTime = 0;
        public static String modelSavePath = ""; // is set during runtime
        public static String communicationPath = "";// is set during runtime
        public static String modelLibPathSimulate = "not given";
        public static String modelLibPathTrain = "not given";
        public static String matlabExecPath = "not given";
        public static uint cPPNInputs = 4;  // ES 4 // TypeCoded =6 // 1D = 3
        public static uint cPPNOutputs = 1; // ES 1 // Typecoded =3 // 1D = 3?
        public static uint numLastGenModelsToPlotAndSave = 0;
        public static uint maxGenerations = 0;
        public static uint populationSize = 0;
        public static uint numDNeurons = 0;
        public static int startAngle = 0;
        public static string simulation_anglesToSimulate = "not given";
        public static int simulation_timePerAngle =0;
        public static String training_anglesToLearn = "not given";
        public static String training_anglesToSimulate = "not given";
        public static int training_timePerAngle =0;
        //angles_to_learn,angles_to_simulate,angle_simulation_time
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
                        case "training_maxTrainTime":
                            training_maxTrainTime = Convert.ToUInt32(line[1]);
                            break;
                        case "numDNeurons":
                            numDNeurons = Convert.ToUInt32(line[1]);
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
                        case "simulation_anglesToSimulate":
                            simulation_anglesToSimulate = line[1];
                            break;
                        case "simulation_timePerAngle":
                            simulation_timePerAngle = Convert.ToInt32(line[1]);
                            break;
                        case "training_anglesToLearn":
                            training_anglesToLearn = line[1];
                            break;
                        case "training_anglesToSimulate":
                            training_anglesToSimulate = line[1];
                            break;
                        case "startAngle":
                            startAngle = Convert.ToInt32(line[1]);
                            break;
                        case "training_timePerAngle":
                            training_timePerAngle = Convert.ToInt32(line[1]);
                            break;
                        default:
                            if (!line[0].StartsWith("//"))
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
