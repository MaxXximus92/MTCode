using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OngoingExperimentNS;
using System.IO;


namespace TestMatlabCommunicator
{
    class TestMatlabCommunicator
    {
        static void Main(string[] args)
        {
            string a=ExperimentParameters.experimentName;
            string matlabpath = ExperimentParameters.matlabPath;
           // string matlabpath = @"C:\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode\ongoingNEAT\matlab";
            MatlabCommunicator mat = new MatlabCommunicator(matlabpath);
            MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
            //mat.matlab.PutWorkspaceData("a", "base", 1);
            mat.matlab.Execute("a = 5;");
            mat2.matlab.Execute("a = 1;");
            double a1 = mat.matlab.GetVariable("a", "base");  // todo maybe wrong base
            double a2 = mat2.matlab.GetVariable("a", "base");
            Console.WriteLine("a1= "+a1);
            Console.WriteLine("a2= " + a2);
            //setMatlabSavePath();
            //mat.generateModel(256,30,ExperimentParameters.matlabSavePath);
            //double[,] weights = mat.getEsEmWeightsMatrix();

            //double[,] newWeights = new double[weights.GetLength(0),weights.GetLength(1)];
            //for(int i=0; i<newWeights.GetLength(0); i++)
            //{
            //    for (int j = 0; j < newWeights.GetLength(1); j++)
            //    {
            //        newWeights[i, j] = 1;
            //    }
            //}
            //mat.setEsEmWeightsMatrix(newWeights);
            //int[] num = mat.getEsEmNum();
            //double[,] weights1 = mat.getEsEmWeightsMatrix();
            //double[,] weights4 = mat.getEsEmWeightsMatrix();
            ////double rmse = mat.startSimulation(ExperimentParameters.runTime, ExperimentParameters.runSettings, ExperimentParameters.trailsPerSetting,true);
            //MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
            //mat2.generateModel(256, 30, ExperimentParameters.matlabSavePath);
            //double[,] weights2 = mat2.getEsEmWeightsMatrix();
            //double[,] weights3 = mat.getEsEmWeightsMatrix(); // verdammt es ist doch der gleiche workspace !!

            //mat.close();
            //    MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
            //  double[,] weightsReturn = mat2.getEsEmWeightsMatrix(); // has own workspcace .. good!
            //  double rmse =  mat.startSimulation();
        }
        static private void setMatlabSavePath()
        {
            string path = Directory.GetCurrentDirectory();
            path = path + @"\matlab";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            ExperimentParameters.matlabSavePath = path;

        }
    }

}
