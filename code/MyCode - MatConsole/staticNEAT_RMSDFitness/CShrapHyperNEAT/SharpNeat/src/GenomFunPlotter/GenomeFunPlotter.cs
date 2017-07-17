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

namespace GenomFunPlotter
{
    class GenomeFunPlotter { 
    static void Main(string[] args)
        {

            int inNeurons = 96; // Convert.ToInt32(args[0]); // oder fest, todo nachschauen!
            int outNeurons = 96; //Convert.ToInt32(args[1]);

            writeCPPNData(getGenomeSavePath(), getCPPNValuesSavePath(),inNeurons,outNeurons);

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


    private static void writeCPPNData(string genomeSavePath, string CPPNValuesSavePath,int inNeurons, int outNeurons)
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

                INetwork net = g.Decode(null);
                double[][] weights = StaticSubstrateForPlot.getFuncValues(net, inNeurons, outNeurons);
                string path = CPPNValuesSavePath + pathSep + name + ".mat";
                MatlabCommunicator.writeSyncMatrix(weights, path);
         }

    }
}
}
