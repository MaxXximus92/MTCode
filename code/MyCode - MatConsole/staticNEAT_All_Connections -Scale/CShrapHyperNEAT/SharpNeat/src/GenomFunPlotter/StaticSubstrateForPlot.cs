using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;
using static StaticExperimentNS.MatlabCommunicator;

namespace GenomFunPlotter
{

    public static class StaticSubstrateForPlot
    {


        public static List<double[][]> getConnectionMatrices(INetwork network, ModelNeuronType[] neuronTypes) //CPPN
        {



            List<double[][]> conMats = new List<double[][]>();

            // build lists with neurontypeindexes
            List<int> DIndexes = getIndexes(ModelNeuronType.D, neuronTypes);
            List<int> EmIndexes = getIndexes(ModelNeuronType.EM, neuronTypes);
            List<int> EsIndexes = getIndexes(ModelNeuronType.ES, neuronTypes);
            List<int> ImIndexes = getIndexes(ModelNeuronType.IM, neuronTypes);
            List<int> IsIndexes = getIndexes(ModelNeuronType.IS, neuronTypes);

            //D zu ES Indixes
           double[][] DEsWeights= getWeights1DModel(DIndexes, EsIndexes, 0, network);
            double[][] EsIsWeights= getWeights1DModel(EsIndexes, IsIndexes, 1, network);
            double[][] IsIsWeights= getWeights1DModel(IsIndexes, IsIndexes, 2, network);
            double[][] IsEsWeights= getWeights1DModel(IsIndexes, EsIndexes, 3,  network);
            double[][] EsEmWeights= getWeights1DModel(EsIndexes, EmIndexes, 4,  network);
            double[][] EmImWeights= getWeights1DModel(EmIndexes, ImIndexes, 5,  network);
            double[][] ImImWeights= getWeights1DModel(ImIndexes, ImIndexes, 6,  network);
            double[][] ImEmWeights= getWeights1DModel(ImIndexes, EmIndexes, 7,  network);

            return new double[][][] { DEsWeights, EsIsWeights, IsIsWeights, IsEsWeights , EsEmWeights , EmImWeights, ImImWeights, ImEmWeights }.ToList();

        }

        private static List<int> getIndexes(ModelNeuronType t, ModelNeuronType[] typeArray)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < typeArray.Length; i++)
            {
                if (t == typeArray[i])
                {
                    indexes.Add(i);
                }
            }
            return indexes;
        }



        /// <summary>
        /// Every heap of same neurons gets represented in a 1d Layer and are placed there on the area [1,0] with same distances. 
        /// Each output of the 8 outputs represents a connection between two moduls.
        /// </summary>
        /// <param name="modul1"></param>
        /// <param name="modul2"></param>
        /// <param name="outputNum"></param> Represents which output is used.
        /// <param name="connections"></param>
        /// <param name="network"></param>
        private static double[][] getWeights1DModel(List<int> modul1, List<int> modul2, int outputNum, INetwork network)
        {

            double weightRange = 1;

            double[][] weights = new double[modul1.Count][];
            for (int i = 0; i < modul1.Count; i++)
            {
                weights[i] = new double[modul2.Count];
            }
            // norm length between 0 and 1
            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;

            float distModul1 = 1.0f / (modul1.Count - 1); // -1 to get a neuron at 1 and at 0
            float distModul2 = 1.0f / (modul2.Count - 1);  // -1 to get a neuron at 1 and at 0

            float[] coordinates = new float[2];
            float mod1coord = 0;
            float mod2coord = 0;

            for (int i = 0; i < modul1.Count; i++)
            {
                mod2coord = 0;
                for (int j = 0; j < modul2.Count; j++)
                {
                    coordinates[0] = mod1coord;
                    coordinates[1] = mod2coord;
                    network.ClearSignals();
                    network.SetInputSignals(coordinates);
                    network.MultipleSteps(iterations);
                    double output = network.GetOutputSignal(outputNum) * weightRange;
                    weights[i][j] = output;

                    mod2coord += distModul2;
                }
                mod1coord += distModul1;
            }

            return weights;


        }

        /// <summary>
        ///  All cells of a type are placed uniform distributed  in a unit square. The left lower corner of the D square is at 0,0 , ES Square = 2,0, EM Square 4,0 IM= 4.0,1.5 and IS = 2,1.5
        /// </summary>
        /// <param name="network"></param>
        /// <param name="neuronTypes"></param>
        /// <returns></returns>
      
    

        private class SPoint
        {
            public float X;
            public float Y;

            public SPoint(float x, float y)
            {
                this.X = x;
                this.Y = y;
            }
        }

      
    }


}



