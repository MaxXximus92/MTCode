using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngoingExperimentNS
{


    public class ExperimentParameters
    {
        public static String experimentName = "no name assigned";
        public static String experimentDescription = "not available";
        public static String matlabPath = "not given";
        public static uint numNeurons = 0;
        //unused in the moment default =30 in matlab code;
        public static uint spikingThreshold = 0;
        public static uint runTime = 0;
        public static string runSettings = "not given";
        // unused -> set to 1 in amtlab code
        public static uint trailsPerSetting =0;
        public static String matlabSavePath = ""; // gets set during runtime
        public static String initialNetWeightsPath = "not given";
        public static String initialNetEqParamsPath = "not given";
        public static uint maxGenerations = 0;
        public static uint populationSize = 0;
        public static uint cPPNInputs = 4;
        public static uint cPPNOutputs = 1;

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
                        case "matlabPath":
                            matlabPath =  line[1];
                            break;
                        case "experimentDescription":
                            experimentDescription =  line[1];
                            break;
                        case "numNeurons":
                            numNeurons = Convert.ToUInt32(line[1]);
                            break;
                        case "spikingThreshold":
                            spikingThreshold = Convert.ToUInt32(line[1]);
                            break;
                        case "runTime":
                            runTime = Convert.ToUInt32(line[1]);
                            break;
                        case "runSettings":
                            runSettings =  line[1];
                            break;
                        case "trailsPerSetting":
                            trailsPerSetting = Convert.ToUInt32(line[1]);
                            break;
                        case "initialNetWeightsPath":
                            initialNetWeightsPath = line[1];
                            break;
                        case "initialNetEqParamsPath":
                            initialNetEqParamsPath = line[1];
                            break;
                        case "maxGenerations":
                            maxGenerations = Convert.ToUInt32(line[1]);
                            break;
                        case "populationSize":
                            populationSize = Convert.ToUInt32(line[1]);
                            break;
                        default:
                            Console.WriteLine(String.Format("unknown Parameter \"{0}\" in experiment settings", line[1]));
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
