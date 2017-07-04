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
using OngoingExperimentNS;

namespace OngoingExperimentNS
{



    class CmdOngoingExperiment
    {
        private static MultiTextWriter multiWriter = new MultiTextWriter();

        static CmdOngoingExperiment()
        {
            TextWriter ConsoleOut = Console.Out;
            FileStream ostrm = new FileStream("./consoleLog1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(ostrm);
            MultiTextWriter multiWriter = new MultiTextWriter();
            multiWriter.AddWriter(writer);
            multiWriter.AddWriter(ConsoleOut);
            Console.SetOut(multiWriter);
        }

    

        static void Main(string[] args)
        {

 
           // string genomeSavePath = "";
            NeatGenome seedGenome = null;
            string filename = null;

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
            Console.WriteLine("Start Experiment: "+ ExperimentParameters.experimentName);


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

            double initialMaxFitness = 0;
            setMatlabSavePath();
            string genomeSavePath = getGenomeSavePath();
            // get initial weights to use
            OngoingModelWrapper model = new OngoingModelWrapper();
            string wpath = Directory.GetCurrentDirectory() + "\\" + ExperimentParameters.initialNetWeightsPath;
            string epath = Directory.GetCurrentDirectory() + "\\" + ExperimentParameters.initialNetEqParamsPath;
            //Console.WriteLine(wpath);
            //Console.WriteLine(epath);
            Console.WriteLine("loading initial data");
            double[,] netWeights = null;
            double[,] equationParams = null;
            int[] esemNum = null;
            model.getModelParams(out netWeights, out equationParams, out esemNum, (int)ExperimentParameters.numNeurons,wpath,epath);

            Console.WriteLine("Creating Experiment");
            OngoingExperiment exp = new OngoingExperiment(cPPNInputs, cPPNOutputs, netWeights, equationParams, esemNum);
           

            StreamWriter SW;
            SW = File.CreateText("logfile.txt");
        
    
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
                Console.WriteLine("------------------------------------\n Generation: "+j+ "\n ------------------------------------");

                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();
                Console.WriteLine("Best fitness of gernatation {0} : {1}",j, ea.BestGenome.Fitness);
                if (ea.BestGenome.Fitness > initialMaxFitness)
                {
                    // Matlab plots speichern = zeitaufwendig da nicht parallelisierbar.  Vllt nur genomes speichern und plots danach machen
                    Console.WriteLine("Save Best Genome");
                    initialMaxFitness = ea.BestGenome.Fitness;
                    saveGenome(ea.BestGenome, genomeSavePath, String.Format("{0}_BestGen_Generation_{1}_Fitness_{2:#####}", ExperimentParameters.experimentName,j,ea.BestGenome.Fitness));
                    // This will output the substrate, uncomment if you want that
                    // gibt resultierendes Netzwerk zur Robotersteuerung zurück -> nicht benötigt
                    // doc = new XmlDocument();
                    // XmlGenomeWriterStatic.Write(doc, (NeatGenome) OnGoingNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                    // oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                    // doc.Save(oFileInfo.FullName);

                  //  1 158,004163310227 00:00:29.9002209 TODO wo kommt die ausgabe her?

                }
                Console.WriteLine("Generation Number{0}, best fitness:{1}, time needed to evaluate: {2}", ea.Generation.ToString() , ea.BestGenome.Fitness , (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                SW.WriteLine(ea.Generation.ToString() + " " + (initialMaxFitness).ToString());

            }
            SW.Close();
            Console.WriteLine("Evolution Done -> SavingResults");
            //get Matlab Plots for best genome

           // save 20 best models of last population

            GenomeList genList = ea.Population.GenomeList;
            genList.Sort(); // increasing values.
            genList.Reverse();
            IGenome[] genArray =genList.ToArray();
            
            for(int i =1; i < 5; i++)
            {
                saveGenome( genArray[i], genomeSavePath, String.Format("{0}_LastGeneration_Fitness_{1:#####}_order_{2}", ExperimentParameters.experimentName,genArray[i].Fitness,i));
            }

            Console.WriteLine("Plotting matlab graphs");
            PlotMatlabGraphs(genomeSavePath, (OnGoingPopulationEvaluator)exp.PopulationEvaluator);

            Console.WriteLine("Done!");
            foreach (TextWriter w in multiWriter.writers)
            {
                w.Flush();
                w.Close();
            }
        
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
            oFileInfo = new FileInfo(genomeSavePath + name);
            doc.Save(oFileInfo.FullName);
        }
        /// <summary>
        /// Loads all saved genomes and simulates them again in matlab to get plots
        /// </summary>
        /// <param name="genomeSavePath"></param>
        private static void PlotMatlabGraphs(string genomeSavePath, OnGoingPopulationEvaluator eva)
        {
            string[] files = Directory.GetFiles(genomeSavePath);
            IGenome[] genomes = new IGenome[files.Length];
            string[] names = new string[files.LongLength];
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i]);
                genomes[i] = XmlNeatGenomeReaderStatic.Read(document);
                names[i] = Path.GetFileNameWithoutExtension(files[i]);  
            }
           eva.evaluateAndMatlabPlot(genomes, names);


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

        static private string getGenomeSavePath()
        {
            string path = Directory.GetCurrentDirectory();
            path = path + @"\genomes\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;

        }


        public class MultiTextWriter : TextWriter
        {
             public List<TextWriter> writers = new List<TextWriter>();

            public override Encoding Encoding => throw new NotImplementedException();


            // ~MultiTextWriter()
            //{
            //    foreach (TextWriter writer in writers)
            //    {
            //        writer.Flush();
            //        writer.Close();
            //    }
            //}
            public void AddWriter(TextWriter writer)
            {
                writers.Add(writer);
            }

            public override void Write(char ch)
            {
                foreach (TextWriter writer in writers)
                {
                    //try
                    //{
                        writer.Write(ch);
                   // writer.Flush();
                    //}
                    //catch (ObjectDisposedException)
                    //{
                    //    // handle exception here
                    //}
                    //catch (IOException)
                    //{
                    //    // handle exception here
                    //}
                }
            }
        }

    }
}
