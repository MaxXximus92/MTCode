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

namespace OngoingExperimentNS
{
    class CmdOngoingExperiment
    {
        static void Main(string[] args)
        {
            string folder = "";
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

            if (filename != null)
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(filename);
                    seedGenome = XmlNeatGenomeReaderStatic.Read(document);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Problem loading genome. \n" + e.Message);
                }
            }

            double maxFitness = 0;
            int maxGenerations = 10;//1000;
            int populationSize = 150;//150;
            int cPPNInputs = 4;
            int cPPNOutputs = 1;
            setMatlabSavePath();
            folder = getGenomeSavePath();
            OnGoingNetworkEvaluator eval = new OnGoingNetworkEvaluator();


            IExperiment exp = new OngoingExperiment(cPPNInputs, cPPNOutputs);
            StreamWriter SW;
            SW = File.CreateText("logfile.txt");
            XmlDocument doc;
            FileInfo oFileInfo;
            IdGenerator idgen;
            EvolutionAlgorithm ea;
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
            for (int j = 0; j < maxGenerations; j++)
            {
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();
                if (ea.BestGenome.Fitness > maxFitness)
                { 
                    //TODO hier auch matlab plots und net speichern !!!
                    maxFitness = ea.BestGenome.Fitness;
                    doc = new XmlDocument();
                    XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                    oFileInfo = new FileInfo(folder + "bestGenome" + j.ToString() + ".xml");
                    doc.Save(oFileInfo.FullName);
                    eval.plotMatlabGraphsAndSafeNetwork(ea.BestGenome.Decode(null), "BestGenome_Generation_" +j.ToString());
                    // This will output the substrate, uncomment if you want that
                    /* doc = new XmlDocument();
                     XmlGenomeWriterStatic.Write(doc, (NeatGenome) SkirmishNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                     oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                     doc.Save(oFileInfo.FullName);
                     */


                }
                Console.WriteLine(ea.Generation.ToString() + " " + ea.BestGenome.Fitness + " " + (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                SW.WriteLine(ea.Generation.ToString() + " " + (maxFitness).ToString());

            }
            SW.Close();

            //get Matlab Plots for best genome

            eval.plotMatlabGraphsAndSafeNetwork(ea.BestGenome.Decode(null),"BestGenome off all"); // act function in  MThreatedPopEvaluator also null ? warum?

            // save best genome
            doc = new XmlDocument();
            XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            oFileInfo = new FileInfo(folder + "bestGenome.xml");
            doc.Save(oFileInfo.FullName);

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
    }
}
