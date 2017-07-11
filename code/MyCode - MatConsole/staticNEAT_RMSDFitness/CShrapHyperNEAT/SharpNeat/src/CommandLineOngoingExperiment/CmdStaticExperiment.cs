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

            Console.WriteLine("test encoding: threshold: "+ HyperNEATParameters.threshold);
            // Console.WriteLine("number of processors: "+ Environment.ProcessorCount);
            // System.Environment.SetEnvironmentVariable("MONO_THREADS_PER_CPU","201");
            //string thredscpu = System.Environment.GetEnvironmentVariable("MONO_THREADS_PER_CPU");

            //   Console.WriteLine("Mono threads per cpu " + thredscpu);

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
            Console.WriteLine("Starting Experiment: "+ ExperimentParameters.experimentName);
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
            Console.WriteLine("loading initial data");
            int[] esemNum =  MatlabCommunicator.getDEsNum("getDEsNum");

            Console.WriteLine("Creating Experiment");
            StaticExperiment exp = new StaticExperiment(cPPNInputs, cPPNOutputs,esemNum);
           

        
    
            IdGenerator idgen;
            EvolutionAlgorithm ea;

            Console.WriteLine("Building Population");
            if (seedGenome == null)
            {
                idgen = new IdGenerator();
                ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(exp.DefaultNeatParameters, idgen, exp.InputNeuronCount, exp.OutputNeuronCount, exp.DefaultNeatParameters.pInitialPopulationInterconnections, populationSize)), exp.PopulationEvaluator, exp.DefaultNeatParameters);

            }
            else
            {
                idgen = new IdGeneratorFactory().CreateIdGenerator(seedGenome);
                ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(seedGenome, populationSize, exp.DefaultNeatParameters, idgen)), exp.PopulationEvaluator, exp.DefaultNeatParameters);
            }
            Console.WriteLine("Run Experiment");
            for (int j = 0; j < maxGenerations; j++)
            {
                Console.WriteLine("------------------------------------\n Generation: "+j+ "\n------------------------------------");
                Population  b= ea.Population;
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();

                Console.WriteLine("Best fitness of gernatation {0} : {1:0.#####}", j, ea.BestGenome.Fitness);
                if (ea.BestGenome.Fitness > initialMaxFitness)
                {
                    Console.WriteLine("Save Best Genome");
                    initialMaxFitness = ea.BestGenome.Fitness;
                    saveGenome(ea.BestGenome, genomeSavePath, String.Format("{0}_BGen_Gen_{1}_Fit_{2:0.#####}", ExperimentParameters.experimentName,j,ea.BestGenome.Fitness));
                    // This will output the substrate, uncomment if you want that
                    // gibt resultierendes Netzwerk zur Robotersteuerung zurück -> nicht benötigt
                    // doc = new XmlDocument();
                    // XmlGenomeWriterStatic.Write(doc, (NeatGenome) OnGoingNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                    // oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                    // doc.Save(oFileInfo.FullName);

                }
                saveGenerationData(ea.Population, (int)ea.Generation, ea.BestGenome.Fitness, getGenerationInfoPath());
                Console.WriteLine("-----------------------------");
                Console.WriteLine("Generation Number{0}, best fitness:{1:0.#####}, time needed to evaluate: {2}", ea.Generation.ToString() , ea.BestGenome.Fitness , (DateTime.Now.Subtract(dt)));
                Console.WriteLine("-----------------------------");
                //Do any post-hoc stuff here
            }

            Console.WriteLine("Evolution Done -> Saving Results");
            //get Matlab Plots for best genome

           // save n best models of last population

            GenomeList genList = ea.Population.GenomeList;
            genList.Sort(); // increasing values.
           // genList.Reverse();
            IGenome[] genArray =genList.ToArray();

            Console.WriteLine("Saving Genomes");
            for (int i = 1; i < Math.Min(ExperimentParameters.numLastGenModelsToPlotAndSave,genArray.Length); i++)
            {
                saveGenome( genArray[i], genomeSavePath, String.Format("{0}_LGen_Fit_{1:0.#####}_ord_{2}", ExperimentParameters.experimentName,genArray[i].Fitness,i));
            }
            Console.WriteLine("Plotting matlab graphs");
            plotMatlabGraphs(genomeSavePath, (StaticPopulationEvaluator)exp.PopulationEvaluator);
            Console.WriteLine("Close training models");
            ((StaticNetworkEvaluator)((MultiThreadedPopulationEvaluator)exp.PopulationEvaluator).networkEvaluator).closeMatlabInstances();
            Console.WriteLine("Simulate model");
            simulateModelAndPlotGraphs(ExperimentParameters.modelSavePath, (StaticPopulationEvaluator)exp.PopulationEvaluator);
            Console.WriteLine("Plotting genome graphs");
            try
            {
                PlotGenomeGraphs(genomeSavePath, getGenomeImageSavePath());
            }
            catch (Exception e) { Console.WriteLine(e.Message); };

            Console.WriteLine("Done!");
            doubleWriter.Flush();
            doubleWriter.Close();

 

        }

        private static void saveGenerationData(Population population, int generation,double bestFitness ,string path)
        {
            float avgComplexity = population.AvgComplexity;
            double meanFitness = population.MeanFitness;
            int popSize = population.PopulationSize;
            int numberSpecies = population.SpeciesTable.Values.Count;
            Species[] species = population.SpeciesTable.Values.Cast<Species>().ToArray<Species>(); ;

            string print = String.Format("Generation = {0} \nMeanFitness = {1} \nBestFitness = {4} \nPopulationSize = {2} \nAvgComplexity= {3} \nAmountSpecies = {5} \n", generation,meanFitness,popSize,avgComplexity,bestFitness,numberSpecies);

            for (int i = 0; i < species.Length; i++)
            {
                Species s = species[i];
                double s_meanfitness = s.MeanFitness;
                int s_id =s.SpeciesId;
                int s_members = s.Members.Count;
                string spec_print = String.Format("--------\n Species = {0} \nId = {1} \nMeanFitness = {2} \nMembers = {3}",i,s_id, s_meanfitness,s_members );
                print += spec_print;
            }
            string fileName = "generation_" + (generation-1) + ".txt";
            StreamWriter writer = new System.IO.StreamWriter(path+pathSep+fileName, false);
            writer.Write(print);
            writer.Flush();
            writer.Close();
        }

        private static void saveGenome(IGenome e, string genomeSavePath, string name)
        {
            XmlDocument doc;
            FileInfo oFileInfo;
            //eval.plotMatlabGraphsAndSafeNetwork(e.Decode(null), name); // act function in  MThreatedPopEvaluator also null ? warum?
            // null cause activation function is defined in each neurons gene
            // save best genome
            doc = new XmlDocument();
            XmlGenomeWriterStatic.Write(doc, (NeatGenome)e);
            oFileInfo = new FileInfo(genomeSavePath +pathSep +name + ".genome");
            doc.Save(oFileInfo.FullName);
        }
        /// <summary>
        /// Loads all saved genomes and simulates them again in matlab to get plots
        /// </summary>
        /// <param name="genomeSavePath"></param>
        private static void plotMatlabGraphs(string genomeSavePath, StaticPopulationEvaluator eva)
        {
            string[] files = Directory.GetFiles(genomeSavePath).Where(s => s.EndsWith(".genome")).ToArray();
            IGenome[] genomes = new IGenome[files.Length];
            string[] names = new string[files.LongLength];
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i]);
                genomes[i] = XmlNeatGenomeReaderStatic.Read(document);
                names[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            eva.trainAndMatlabPlot(genomes, names);
  

        }

        private static void simulateModelAndPlotGraphs(string modelSavePath, StaticPopulationEvaluator eva)
        {
            string[] files = Directory.GetFiles(modelSavePath).Where(s => s.EndsWith("aftsim.mat")).ToArray();
            string[] names = new string[files.LongLength];
            for (int i = 0; i < files.Length; i++)
            {
                names[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
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
               string print =  DateTime.Now.ToString("dd HH:mm:ss ")+value;
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
