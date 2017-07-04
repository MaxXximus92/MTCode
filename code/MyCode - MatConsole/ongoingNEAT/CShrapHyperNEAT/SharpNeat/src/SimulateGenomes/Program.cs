using OngoingExperimentNS;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static OngoingExperimentNS.Helper;

namespace SimulateGenomes
{

  
    class SimulateGenomes
    {
        static SimulateGenomes(){
        setCommunicationPath();
        setMatlabSavePath();

    }

    static void Main(string[] args)
        {
            int cPPNInputs = (int)ExperimentParameters.cPPNInputs;
            int cPPNOutputs = (int)ExperimentParameters.cPPNOutputs;
            Console.WriteLine("loading initial data");
            int[] esemNum = MatlabCommunicator.getEsEMNum("getEsEMNum");

            Console.WriteLine("Creating Experiment");
            OngoingExperiment exp = new OngoingExperiment(cPPNInputs, cPPNOutputs, esemNum);
            Console.WriteLine("Plotting Graphs");
            PlotMatlabGraphs(getGenomeSavePath(), (OnGoingPopulationEvaluator)exp.PopulationEvaluator, "Simulate");


        }
        private static void PlotMatlabGraphs(string genomeSavePath, OnGoingPopulationEvaluator eva, string namePrefix)
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
            eva.evaluateAndMatlabPlot(genomes, names, namePrefix);
            Console.WriteLine("Saving Fitness");
            saveFitness(genomes, names, genomeSavePath);
            Console.WriteLine("done");
            // write genome fitness to file


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
        private static void saveFitness(IGenome[] genomes,string[] names, string path)
        {
            string print = "";
            for (int i = 0; i < genomes.Length; i++)
            {

                double fitness = genomes[i].Fitness;
                string name = names[i];
                print += String.Format(" Name: {0} \nFitness = {1} \n ", name,fitness);
            }

            string fileName = "fitnesses.txt";
            StreamWriter writer = new System.IO.StreamWriter(path + pathSep + fileName, false);
            writer.Write(print);
            writer.Flush();
            writer.Close();
        }

    static private void setMatlabSavePath()
    {
        string path = toFullPath(@"matlabSave");
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

}
}
