using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static EsExperimentNS.EsSubstrate;
using static EsExperimentNS.Helper;

namespace EsExperimentNS
{
    class FunctionPlot1D
    {
        static void Main(string[] args)
        {

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            writeCPPNData(getGenomeSavePath(), getCPPNValuesSavePath());

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


        private static void writeCPPNData(string genomeSavePath, string CPPNValuesSavePath)
        {
            string[] files = Directory.GetFiles(genomeSavePath).Where(s => s.EndsWith(".genome")).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine("Parsing: " + files[i]);
                XmlDocument document = new XmlDocument();
                document.Load(files[i]);
                IGenome g = XmlNeatGenomeReaderStatic.Read(document);
                string name = Path.GetFileNameWithoutExtension(files[i]);

                int resolution = 1024;
                INetwork net = g.Decode(null);
                List < EsSubstrate1DForPlot.Neuron > neurons=null;
                EsSubstrate1DForPlot substrate = new EsSubstrate1DForPlot(net);
                EsSubstrate1D s2 = new EsSubstrate1D(net);

                List<NType> types=null;
                int[][] w1= s2.getConnections(out types);
                double[][] connectionWeights = substrate.GetConnections( out neurons);
                double[][] cppnWeightValues = substrate.scanValues(resolution);
                double[] cppnTypeCodeValues1 = substrate.getNeuronCodingValues(1,resolution);
                double[] cppnTypeCodeValues2 = substrate.getNeuronCodingValues(2, resolution);
                double[] cppnTypeCodeValues3 = substrate.getNeuronCodingValues(3, resolution);
                double[] cppnTypeCodeNeurons1 = substrate.getNeuronCodingValues(1, neurons);
                double[] cppnTypeCodeNeurons2 = substrate.getNeuronCodingValues(2, neurons);
                double[] cppnTypeCodeNeurons3 = substrate.getNeuronCodingValues(3, neurons);
                double[] neuronsPos = neurons.Select(s => (double)s.pos).ToArray();
                double[] neuronsType = neurons.Select(s => (double)((int)s.type)).ToArray();

                string path = CPPNValuesSavePath + pathSep + name + ".mat";
                MatlabCommunicator.writePlotData(connectionWeights, cppnWeightValues, cppnTypeCodeValues1, cppnTypeCodeValues2, cppnTypeCodeNeurons1, cppnTypeCodeNeurons2, neuronsPos, neuronsType, path, cppnTypeCodeValues3, cppnTypeCodeNeurons3);
            }

        }
    }
}
