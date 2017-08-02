using csmatio.io;
using csmatio.types;
using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NetworkVisualization;
using SharpNeatLib.NeuralNetwork;
using StaticExperimentNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static StaticExperimentNS.Helper;
using static StaticExperimentNS.MatlabCommunicator;

namespace GenomFunPlotter
{
    class GenomeFunPlotter { 
    static void Main(string[] args)
        {


            ModelNeuronType[] neuronTypes = getTypes("neuronTypes.mat"); ;

            writeCPPNData(getGenomeSavePath(), getCPPNValuesSavePath(), neuronTypes);

        }

    private static  ModelNeuronType[] getTypes(string path)
        {
            MatFileReader reader = new MatFileReader(path);
            MLDouble result = (MLDouble)reader.GetMLArray("neuronTypes");
            double[] types = result.GetArray()[0];
            ModelNeuronType[] enumTypes = new ModelNeuronType[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                enumTypes[i] = (ModelNeuronType)Convert.ToInt32(types[i]);
            }
            Console.WriteLine("loaded types' length:" + types.Length);
            return enumTypes;
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

    static private string getCPPNValuesSavePath()
    {
        string path = toFullPath(@"CPPNValues");
        // string path = Directory.GetCurrentDirectory();
        //path = path + pathSep+@"genomeImages"+ pathSep;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;

    }


    private static void writeCPPNData(string genomeSavePath, string CPPNValuesSavePath, ModelNeuronType[] neuronTypes)
    {
        string[] files = Directory.GetFiles(genomeSavePath).Where(s => s.EndsWith(".genome")).ToArray();
        GridLayoutManager gridManager = new GridLayoutManager();
        NetworkControl control = new NetworkControl();
        for (int i = 0; i < files.Length; i++)
        {
            XmlDocument document = new XmlDocument();
            document.Load(files[i]);
            IGenome g = XmlNeatGenomeReaderStatic.Read(document);
            string name = Path.GetFileNameWithoutExtension(files[i]);

                INetwork net = g.Decode(null);
                List<double[][]> weights = StaticSubstrateForPlot.getConnectionMatrices(net, neuronTypes);
                string path = CPPNValuesSavePath + pathSep + name + ".mat";
                MatlabCommunicator.writeWeightMatrices(weights, path);
         }

    }
}
}
