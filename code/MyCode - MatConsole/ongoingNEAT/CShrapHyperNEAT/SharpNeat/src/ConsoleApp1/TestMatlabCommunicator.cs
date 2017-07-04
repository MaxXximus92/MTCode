using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OngoingExperimentNS;
using System.IO;
using MatlabModel;
using MathWorks.MATLAB.NET.Arrays;
using static OngoingExperimentNS.Helper;
using SharpNeatLib.NetworkVisualization;
using System.Xml;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.Evolution;
using SharpNeatLib;
using SharpNeatLib.NeatGenome;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace TestMatlabCommunicator
{
    class TestMatlabCommunicator {

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PlotGenomeGraphs(getGenomeSavePath(), getGenomeImageSavePath());


          //   setCommunicationPath();
          //   setMatlabSavePath();


            //  DateTime dt2 = DateTime.Now;
            //  int[] esem = MatlabCommunicator.getEsEMNum("mat_1");
            //  Console.WriteLine("time needed_ params " + (DateTime.Now.Subtract(dt2)));

            //double[][] weights = new double[esem[0]][];
            //  for (int i = 0; i < esem[0]; i++)
            //  {
            //      weights[i] = new double[esem[1]];
            //      for (int j = 0; j < esem[1]; j++)
            //      {
            //          weights[i][j] = i + j;
            //      }
            //  }
            //  MatlabCommunicator mat = new MatlabCommunicator("mat_2");
            //  //MatlabCommunicator mat2 = new MatlabCommunicator("mat_3");
            //  DateTime dt = DateTime.Now;
            //  //double rmsd1 = mat.simulate(weights);
            //  //Console.WriteLine("time needed_ 1 " + (DateTime.Now.Subtract(dt)));
            //  // dt = DateTime.Now;
            //  double rmsd2 = mat.simulateWithPlot(weights,"TestName");
            //  Console.WriteLine("time needed_ 2 " + (DateTime.Now.Subtract(dt)));
            //dt = DateTime.Now;
            //double rmsd3 = mat.simulate(weights);
            //Console.WriteLine("time needed_ 3 " + (DateTime.Now.Subtract(dt)));
            //dt = DateTime.Now;
            //double rmsd4 = mat2.simulate(weights);
            //Console.WriteLine("time needed_ 4 " + (DateTime.Now.Subtract(dt)));


            //Console.WriteLine(rmsd1 + "rmsd 1 -----------------------");
            //Console.WriteLine(rmsd2 + "rmsd 2 -----------------------");
            //Console.WriteLine(rmsd3 + "rmsd 3-----------------------");
            //Console.WriteLine(rmsd4 + "rmsd 4-----------------------");
        }
    //        string name = ExperimentParameters.experimentName;
    //        string matlabpath = ExperimentParameters.matlabPath;
    //        // MatlabCommunicator mat = new MatlabCommunicator(matlabpath);
    //        //double[,] weights = mat.getNetWeightsMatrix();
    //        int runtime = 40000;
    //        string runSettings = "[65;35]";
    //        int numNeurons = 256;
    //        string savepath= @"C:\Users\Maximus Mutschler\Downloads\test";
    //        bool isSave = false;
    //        string weightPath = @"matlab\weights.mat";
    //        string eqParamsPath= @"matlab\eqparams.mat";

     
    //        MWCharArray m_weightPath = new MWCharArray(weightPath);
    //        MWCharArray m_eqParamsPath = new MWCharArray(eqParamsPath);
    //        MWNumericArray m_numNeurons = new MWNumericArray(numNeurons);

    //        //MWArray[] result = model.getModelParams(3, m_numNeurons, m_weightPath, m_eqParamsPath);
    //        double[,] weightsM ;
    //        double[,] equationParams; 
    //        int[] esemNum ;
    //        DateTime dt2 = DateTime.Now;
    //        OngoingModelWrapper model = new OngoingModelWrapper();
    //        Console.WriteLine("time1" + (DateTime.Now.Subtract(dt2)));
    //        model.getModelParams(out weightsM,out equationParams, out esemNum,numNeurons, weightPath,eqParamsPath);

    //        double[,] esemWeights = new double[esemNum[0], esemNum[1]];
    //        DateTime dt = DateTime.Now;
    //        MWArray rmsd = model.runModel(name, numNeurons, runtime,runSettings, savepath, equationParams, weightsM, esemWeights, isSave);
    //     //   model.runModel(name, numNeurons, runtime, runSettings, savepath, equationParams, weightsM, esemWeights, isSave);
    //     //   model.runModel(name, numNeurons, runtime, runSettings, savepath, equationParams, weightsM, esemWeights, isSave);
    //     //   model.runModel(name, numNeurons, runtime, runSettings, savepath, equationParams, weightsM, esemWeights, isSave);
    //        Console.WriteLine("time" + (DateTime.Now.Subtract(dt)));
    //        double rmsdval = ((double[,])rmsd.ToArray())[0, 0]; ;
    //        int a = 2;

    //    }
        //static void Main(string[] args)
        //{
        //    string a=ExperimentParameters.experimentName;
        //    string matlabpath = ExperimentParameters.matlabPath;
        //   // string matlabpath = @"C:\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode\ongoingNEAT\matlab";
        //    MatlabCommunicator mat = new MatlabCommunicator(matlabpath);
        //    setMatlabSavePath();
        //    mat.generateModel(256, 30, ExperimentParameters.matlabSavePath);
        //    mat.startSimulation();

        //   // double[,] weights = mat.getNetWeightsMatrix();

        //   // double[,] newWeights = new double[weights.GetLength(0), weights.GetLength(1)];
        //   // for (int i = 0; i < newWeights.GetLength(0); i++)
        //   // {
        //   //     for (int j = 0; j < newWeights.GetLength(1); j++)
        //   //     {
        //   //         newWeights[i, j] = 1;
        //   //     }
        //   // }
        //   // double[,] equationParams = mat.getNetEquationparams();

        //   // double[,] newequationParams = new double[equationParams.GetLength(0), equationParams.GetLength(1)];
        //   // for (int i = 0; i < newequationParams.GetLength(0); i++)
        //   // {
        //   //     for (int j = 0; j < newequationParams.GetLength(1); j++)
        //   //     {
        //   //         newequationParams[i, j] = 1;
        //   //     }
        //   // }
        //   // string weightspath = ExperimentParameters.matlabSavePath + @"\weights.mat";
        //   // string eqpath= ExperimentParameters.matlabSavePath + @"\eqparams.mat";
        //   // File.Delete(eqpath);
        //   // File.Delete(weightspath);

        //   // mat.saveEquationParams(equationParams, eqpath);
        //   // mat.saveNetWeights(weights, weightspath);

        //   // newequationParams = mat.loadEquationParams(eqpath);
        //   // newWeights = mat.loadNetWeights(weightspath);

        //   //// File.Delete(eqpath);
        //   //// File.Delete(weightspath);
        //   // MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
        //   // mat2.generateModel(newWeights, newequationParams, 256, 30, ExperimentParameters.matlabSavePath);

        //   // double[,] weights2 = mat2.getNetWeightsMatrix();
        //   // double[,] equationParams2 = mat2.getNetEquationparams();

        //   // double[,] Wes =mat2.getEsEmWeightsMatrix();
        //   // for (int i = 0; i < Wes.GetLength(0); i++)
        //   // {
        //   //     for (int j = 0; j < Wes.GetLength(1); j++)
        //   //     {
        //   //         Wes[i, j] = 1;
        //   //     }
        //   // }
        //   // mat2.setEsEmConnectionsMatrix(Wes);
        //   // double[,] weights3 = mat2.getEsEmWeightsMatrix();





        //    //MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
        //    //mat.matlab.PutWorkspaceData("a", "base", 1);
        //    //mat.matlab.Execute("a = 5;");
        //    // mat2.matlab.Execute("a = 1;");
        //    // double a1 = mat.matlab.GetVariable("a", "base");  // todo maybe wrong base
        //    // double a2 = mat2.matlab.GetVariable("a", "base");
        //    // Console.WriteLine("a1= "+a1);
        //    //  Console.WriteLine("a2= " + a2);
        //    //setMatlabSavePath();
        //    //mat.generateModel(256,30,ExperimentParameters.matlabSavePath);
        //    //double[,] weights = mat.getEsEmWeightsMatrix();

        //    //double[,] newWeights = new double[weights.GetLength(0),weights.GetLength(1)];
        //    //for(int i=0; i<newWeights.GetLength(0); i++)
        //    //{
        //    //    for (int j = 0; j < newWeights.GetLength(1); j++)
        //    //    {
        //    //        newWeights[i, j] = 1;
        //    //    }
        //    //}
        //    //mat.setEsEmWeightsMatrix(newWeights);
        //    //int[] num = mat.getEsEmNum();
        //    //double[,] weights1 = mat.getEsEmWeightsMatrix();
        //    //double[,] weights4 = mat.getEsEmWeightsMatrix();
        //    ////double rmse = mat.startSimulation(ExperimentParameters.runTime, ExperimentParameters.runSettings, ExperimentParameters.trailsPerSetting,true);
        //    //MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
        //    //mat2.generateModel(256, 30, ExperimentParameters.matlabSavePath);
        //    //double[,] weights2 = mat2.getEsEmWeightsMatrix();
        //    //double[,] weights3 = mat.getEsEmWeightsMatrix(); // verdammt es ist doch der gleiche workspace !!

        //    //mat.close();
        //    //    MatlabCommunicator mat2 = new MatlabCommunicator(matlabpath);
        //    //  double[,] weightsReturn = mat2.getEsEmWeightsMatrix(); // has own workspcace .. good!
        //    //  double rmse =  mat.startSimulation();
        //}
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

        static private void setCommunicationPath()
        {
            string path = toFullPath(@"communication");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            ExperimentParameters.communicationPath = path;

        }

        //        int[][] jagged = new int[arr.GetLength(0)][];

        //for (int i = 0; i<arr.GetLength(0); i++)
        //{
        //    jagged[i] = new int[arr.GetLength(1)];
        //    for (int j = 0; j<arr.GetLength(1); j++)
        //    {
        //        jagged[i][j] = arr[i, j];
        //    }
        //}



        static private string getGenomeSavePath()
        {
            string path = toFullPath(@"genomes");
            //Directory.GetCurrentDirectory();
            // path = path + pathSep+@"genomes"+ pathSep;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;

        }

        static private string getGenomeImageSavePath()
        {
            string path = toFullPath(@"genomeImages");
            // string path = Directory.GetCurrentDirectory();
            //path = path + pathSep+@"genomeImages"+ pathSep;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;

        }


        private static void PlotGenomeGraphs(string genomeSavePath, string GenomeImageSavePath)
        {
            string[] files = Directory.GetFiles(genomeSavePath).Where(s => s.EndsWith(".genome")).ToArray();
            GridLayoutManager gridManager = new GridLayoutManager();
            NetworkControl control = new NetworkControl();
            NetworkModel model;
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i]);
                IGenome g = XmlNeatGenomeReaderStatic.Read(document);
                string name = Path.GetFileNameWithoutExtension(files[i]);
                model = GenomeDecoder.DecodeToNetworkModel((NeatGenome)g);
                
                gridManager.Layout(model, control.Size); // changed from model.size
                control.NetworkModel = model;
                
                using (Graphics gfx = control.CreateGraphics())
                {

                    using (Bitmap bmp = new Bitmap(control.Viewport.image))
                    {
                        bmp.Save(GenomeImageSavePath + pathSep + name + ".bmp");

                    }
                }

            }

        }
    }




}
