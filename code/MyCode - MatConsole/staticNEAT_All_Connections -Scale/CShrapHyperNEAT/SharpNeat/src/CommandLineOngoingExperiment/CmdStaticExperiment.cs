using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using SharpNeatLib.AppConfig;
using SharpNeatLib.Evolution;
using SharpNeatLib.Evolution.Xml;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;
using StaticExperimentNS;
using SharpNeatLib.NetworkVisualization;
using SharpNeatLib;
using System.Drawing;
using static StaticExperimentNS.Helper;
using System.Collections;
using System.Threading;
using System.Globalization;
using static StaticExperimentNS.MatlabCommunicator;

namespace StaticExperimentNS
{



    class CmdOngoingExperiment
    {
        private static DoubleWriter doubleWriter = null;

        static CmdOngoingExperiment()
        {
            // important for HyperneatLib to load data correct
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            TextWriter ConsoleOut = Console.Out;

            FileStream ostrm = new FileStream("./consoleLog.txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(ostrm);
            writer.AutoFlush = true;
            doubleWriter = new DoubleWriter(writer, ConsoleOut);
            Console.SetOut(doubleWriter);
            setCommunicationPath();
            setModelSavePath();

        }



        static void Main(string[] args)
        {

 
           // string genomeSavePath = "";
            NeatGenome seedGenome = null;
            string filename = null;

            Console.WriteLine("test encoding: threshold: " + HyperNEATParameters.threshold);
            // Console.WriteLine("number of processors: "+ Environment.ProcessorCount);
            //System.Environment.SetEnvironmentVariable("MONO_THREADS_PER_CPU","201");
            //string thredscpu = System.Environment.GetEnvironmentVariable("MONO_THREADS_PER_CPU");

            //            Console.WriteLine("Mono threads per cpu " + thredscpu);

            //for (int j = 0; j < args.Length; j++)
            //{
            //    if (j <= args.Length - 2)
            //        switch (args[j])
            //        {
            //            case "-seed":
            //                filename = args[++j];
            //                Console.WriteLine("Attempting to use seed from file " + filename);
            //                break;
            //            case "-folder":
            //                folder = args[++j];
            //                Console.WriteLine("Attempting to output to folder " + folder);
            //                break;
            //        }
            //}

            //if (filename != null)
            //{
            //    try
            //    {
            //        XmlDocument document = new XmlDocument();
            //        document.Load(filename);
            //        seedGenome = XmlNeatGenomeReaderStatic.Read(document);
            //    }
            //    catch (Exception e)
            //    {
            //        System.Console.WriteLine("Problem loading genome. \n" + e.Message);
            //    }
            //}
            Console.WriteLine("Starting Experiment: " + ExperimentParameters.experimentName);
            Console.WriteLine("--------------------------------------------------------");


            //int maxGenerations = 1;//1000;
            //int populationSize = 6;//150; -> has to be >=20
            //int cPPNInputs = 4;
            //int cPPNOutputs = 1;
            //bool bias = false;


            int maxGenerations = (int)ExperimentParameters.maxGenerations;
            int populationSize = (int)ExperimentParameters.populationSize;
            int cPPNInputs =(int) ExperimentParameters.cPPNInputs;
            int cPPNOutputs =(int) ExperimentParameters.cPPNOutputs;
            // bool bias =  ExperimentParameters.biasNeuron; CPPN hat default mäßig ein bias neuron -> siehe Genome Factory

            double initialMaxFitness = -1;
            string genomeSavePath = getGenomeSavePath();
            // get initial weights to use
            //string wpath = Directory.GetCurrentDirectory() + "\\" + ExperimentParameters.initialNetWeightsPath;
            //string epath = Directory.GetCurrentDirectory() + "\\" + ExperimentParameters.initialNetEqParamsPath;
            //Console.WriteLine(wpath);
            //Console.WriteLine(epath);
            string factors = ExperimentParameters.scaleFactors;
            string[] factorArray = factors.Split(';').ToArray();

            for (int i = 0; i < factorArray.Length; i++)
            {
            ExperimentParameters.scaleFactors = factorArray[i];

            Console.WriteLine("loading initial data for " + factorArray[i]);
            ModelNeuronType[] neuronTypes =  MatlabCommunicator.getNeuronTypes("getNeuronTypes");

            Console.WriteLine("Creating Experiment");
            StaticExperiment exp = new StaticExperiment(cPPNInputs, cPPNOutputs, neuronTypes);

            //get Matlab Plots for best genome

           // save n best models of last population

            Console.WriteLine("Plotting matlab graphs");
            plotMatlabGraphs(genomeSavePath, (StaticPopulationEvaluator)exp.PopulationEvaluator, factorArray[i]);
            Console.WriteLine("Close training models");
            ((StaticNetworkEvaluator)((MultiThreadedPopulationEvaluator)exp.PopulationEvaluator).networkEvaluator).closeMatlabInstances();
            Console.WriteLine("Simulate model");
            simulateModelAndPlotGraphs(ExperimentParameters.modelSavePath, (StaticPopulationEvaluator)exp.PopulationEvaluator, factorArray[i]);

            }

            Console.WriteLine("Done!");
            doubleWriter.Flush();
            doubleWriter.Close();

 

        }

   

        /// <summary>
        /// Loads all saved genomes and simulates them again in matlab to get plots
        /// </summary>
        /// <param name="genomeSavePath"></param>
        private static void plotMatlabGraphs(string genomeSavePath, StaticPopulationEvaluator eva, string postfix)
        {
            string[] files = Directory.GetFiles(genomeSavePath).Where(s => s.EndsWith(".genome")).ToArray();
            IGenome[] genomes = new IGenome[files.Length];
            string[] names = new string[files.LongLength];
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i]);
                genomes[i] = XmlNeatGenomeReaderStatic.Read(document);
                names[i] = postfix + "-"+Path.GetFileNameWithoutExtension(files[i]);
            }
            eva.evaluateAndMatlabPlot(genomes, names);
  

        }

        private static void simulateModelAndPlotGraphs(string modelSavePath, StaticPopulationEvaluator eva, string prefix)
        {

            string[] names = Directory.GetFiles(modelSavePath).Select(s => Path.GetFileNameWithoutExtension(s)).Where(s => s.EndsWith("aftsim") && s.StartsWith(prefix)).ToArray();
           // string[] files = Directory.GetFiles(modelSavePath).Where(s => s.EndsWith("aftsim.mat") && s.StartsWith(prefix)).ToArray();

            eva.simulateAndPlot(names);

        }



        static string getGenerationInfoPath()
        {
            string path = toFullPath(@"generationInfo");
            //Directory.GetCurrentDirectory();
            // path = path + pathSep+@"genomes"+ pathSep;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        static private void setModelSavePath()
        {
            string path = toFullPath(@"matlabSave");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            ExperimentParameters.modelSavePath = path;

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
            string path = toFullPath( @"genomeImages");
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
                gridManager.Layout(model, control.Size);
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




        class DoubleWriter : TextWriter
        {

            TextWriter one;
            TextWriter two;
            Semaphore sem = new Semaphore(1, 1);
            public DoubleWriter(TextWriter one, TextWriter two)
            {
                this.one = one;
                this.two = two;
            }

            public override Encoding Encoding
            {
                get { return one.Encoding; }
            }

            public override void Flush()
            {
                sem.WaitOne();
                one.Flush();
                two.Flush();
                sem.Release();
            }


            public override void WriteLine(string value)
            {
                string print = DateTime.Now.ToString("dd HH:mm:ss ") + value;
                base.WriteLine(print);
            }

            public override void Write(char value)
            {
                sem.WaitOne();
                one.Write(value);
                two.Write(value);
                sem.Release();
            }

            public override void Close()
            {
                one.Close();
                two.Close();
            }

        }


    }
}
