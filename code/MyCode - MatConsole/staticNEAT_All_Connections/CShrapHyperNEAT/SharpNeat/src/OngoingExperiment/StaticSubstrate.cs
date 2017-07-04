using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;
using static StaticExperimentNS.MatlabCommunicator;
using System.Drawing;

namespace StaticExperimentNS
{

    static class StaticSubstrate
    {



    public static double[][] getConnections1DModel(INetwork network, ModelNeuronType[] neuronTypes) //CPPN
        {
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange;

            int matrixLenght = neuronTypes.Length;
            double[][] connections = new double[matrixLenght][];
            for (int i = 0; i < matrixLenght; i++)
            {
                connections[i] = new double[matrixLenght];
            }

            // build lists with neurontypeindexes
            List<int> DIndexes = getIndexes(ModelNeuronType.D, neuronTypes);
            List<int> EmIndexes = getIndexes(ModelNeuronType.EM, neuronTypes);
            List<int> EsIndexes = getIndexes(ModelNeuronType.ES, neuronTypes);
            List<int> ImIndexes = getIndexes(ModelNeuronType.IM, neuronTypes);
            List<int> IsIndexes = getIndexes(ModelNeuronType.IS, neuronTypes);

            //D zu ES Indixes
            setConnections1DModel(DIndexes, EsIndexes, 0,ref connections, network);
            setConnections1DModel(EsIndexes, IsIndexes, 1, ref connections, network);
            setConnections1DModel(IsIndexes, IsIndexes, 2, ref connections, network);
            setConnections1DModel(IsIndexes, EsIndexes, 3, ref connections, network);
            setConnections1DModel(EsIndexes, EmIndexes, 4, ref connections, network);
            setConnections1DModel(EmIndexes, ImIndexes, 5, ref connections, network);
            setConnections1DModel(ImIndexes, ImIndexes, 6, ref connections, network);
            setConnections1DModel(ImIndexes, EmIndexes, 7, ref connections, network);

     
            return connections;
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
        private static void setConnections1DModel(List<int> modul1, List<int> modul2, int outputNum,ref double[][] connections, INetwork network)
        {
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange;
            // norm length between 0 and 1
            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;

            float distModul1 = 1.0f/(modul1.Count -1); // -1 to get a neuron at 1 and 0
            float distModul2 = 1.0f /( modul2.Count-1);  // -1 to get a neuron at 1 and 0

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
                    double output = network.GetOutputSignal(outputNum)*weightRange;

                    if (output >=threshold)
                    {
                        connections[modul1[i]][modul2[j]] = 1;
                    }
                    mod2coord += distModul2;
                }
                mod1coord += distModul1;
            }



           
        }

        /// <summary>
        ///  All cells of a type are placed uniform distributed  in a unit square. The left lower corner of the D square is at 0,0 , ES Square = 2,0, EM Square 4,0 IM= 4.0,1.5 and IS = 2,1.5
        /// </summary>
        /// <param name="network"></param>
        /// <param name="neuronTypes"></param>
        /// <returns></returns>
        public static double[][] getConnectionsSquareModel(INetwork network, ModelNeuronType[] neuronTypes)
        {
  

            int matrixLenght = neuronTypes.Length;
            double[][] connections = new double[matrixLenght][];
            for (int i = 0; i < matrixLenght; i++)
            {
                connections[i] = new double[matrixLenght];
            }

            Dictionary<int, SPoint> indexToPointMap = new Dictionary<int, SPoint>();

            List<int> DIndexes = getIndexes(ModelNeuronType.D, neuronTypes);
            List<int> EmIndexes = getIndexes(ModelNeuronType.EM, neuronTypes);
            List<int> EsIndexes = getIndexes(ModelNeuronType.ES, neuronTypes);
            List<int> ImIndexes = getIndexes(ModelNeuronType.IM, neuronTypes);
            List<int> IsIndexes = getIndexes(ModelNeuronType.IS, neuronTypes);

            createSquare(DIndexes, indexToPointMap);
            createSquare(EmIndexes, indexToPointMap);
            createSquare(EsIndexes, indexToPointMap);
            createSquare(ImIndexes, indexToPointMap);
            createSquare(IsIndexes, indexToPointMap);

            move(EmIndexes, indexToPointMap, 4, 0);
            move(EsIndexes, indexToPointMap, 2, 0);
            move(ImIndexes, indexToPointMap, 2, 1.5f);
            move(IsIndexes, indexToPointMap, 2, -1.5f);

            // TODO set connctions method

            setConnectionsSquareModel(DIndexes, EsIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(EsIndexes, IsIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(IsIndexes, IsIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(IsIndexes, EsIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(EsIndexes, EmIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(EmIndexes, ImIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(ImIndexes, ImIndexes, indexToPointMap, ref connections, network);
            setConnectionsSquareModel(ImIndexes, EmIndexes, indexToPointMap, ref connections, network);

            return connections;
        }
        private static void setConnectionsSquareModel(List<int> modul1, List<int> modul2, Dictionary<int, SPoint> indexToPointMap, ref double[][] connections, INetwork network)
        {

            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange;
            float[] coordinates = new float[4];
            foreach (int indexMod1 in modul1)
            {
                foreach (int indexMod2 in modul2)
                {
                    SPoint p1 = indexToPointMap[indexMod1];
                    SPoint p2 = indexToPointMap[indexMod2];
                    coordinates[0] = p1.X;
                    coordinates[1] = p1.Y;
                    coordinates[2] = p2.X;
                    coordinates[3] = p2.Y;

                    network.ClearSignals();
                    network.SetInputSignals(coordinates);
                    network.MultipleSteps(iterations);
                    double output = network.GetOutputSignal(0)* weightRange;

                    if (output >= threshold)
                    {
                        connections[indexMod1][indexMod2] = 1;
                    }

                }

            }
        }

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

        private static void createSquare(List<int> modul, Dictionary<int, SPoint> indexToPointMap)
        {
            int neurons = modul.Count;
            int neurons1Direction = Convert.ToInt32(Math.Ceiling(Math.Sqrt(neurons)));
            float gap = 1.0f / (neurons1Direction-1); // -1 to get a neuron at 1 and 0

            float x = 0;
            float y = 0;

            int neuronCounter=0;
            for (int i = 0; i < neurons1Direction; i++)
            {
                x = 0;
                for (int j = 0; j < neurons1Direction; j++)
                {
                    indexToPointMap.Add(modul[neuronCounter],new SPoint(x,y));
                    neuronCounter++;
                    if (neuronCounter == neurons)
                    {
                        return;
                    } 
                    x += gap;
                }
                y += gap;

            }
        }/// <summary>
        /// Moves down left corner to this position
        /// </summary>
        /// <param name="modul"></param>
        /// <param name="indexToPointMap"></param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        private static void move(List<int> modul, Dictionary<int, SPoint> indexToPointMap, float posx, float posy)
        {
            foreach (int index in modul)
            {
                SPoint p = indexToPointMap[index]; // is call by reference
                p.X = p.X + posx;
                p.Y = p.Y + posy;
            }
        }
    }


}
