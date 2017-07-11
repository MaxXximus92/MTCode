using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;
using SharpNeatLib.CPPNs;
using System.Diagnostics;
using System.Drawing;

namespace EsExperimentNS
{

    public class EsSubstrate : EvolvableSubstrate
    {
        public EsSubstrate(INetwork genome)
        {
            this.varianceThreshold = (float)HyperNEATParameters.varianceThreshold;
            this.bandThreshold = (float)HyperNEATParameters.bandingThreshold;
            this.divisionThreshold = (float)HyperNEATParameters.divisionThreshold;
            this.maxDepth = HyperNEATParameters.maximumDepth;
            this.genome = (ModularNetwork)genome;
            this.initialDepth = HyperNEATParameters.initialDepth;

        }
        public enum NType { D, EM, ES, IM, IS };

        internal class Neuron
        {
            public NType type;
            public PointF point;
            public List<Neuron> outCon = new List<Neuron>();
            public List<Neuron> inCon = new List<Neuron>();
            public bool toDelete = false;
            public int index = -1;
            public Neuron(NType type, PointF point)
            {
                this.type = type;
                this.point = point;
            }


        }

        public int[][] getConnections(out List<NType> types)
        {
            uint numInputNeurons = ExperimentParameters.numDNeurons;
            // to HyperNeatParameter
           // int genomeIterations = HyperNEATParameters.ESIterations;

            Dictionary<PointF, Neuron> neuronDictionary = new Dictionary<PointF, Neuron>();


            List<Neuron[]> connections = new List<Neuron[]>(); //int 2 input output

            //Place Input Neurons y=0 x in area between 0 and 1
            List<Neuron> inputNeurons = new List<Neuron>();
            float inputDelta = 1f / (numInputNeurons - 1);
            float xPos = 0;
            for (int i = 0; i < numInputNeurons; i++)
            {
                PointF point = new PointF(xPos, 0);
                Neuron n = new Neuron(NType.D, point);
                inputNeurons.Add(n);
                neuronDictionary.Add(point, n);
                xPos += inputDelta;
            }


            List<Neuron> esNeurons = scanForNewConnections(inputNeurons, NType.ES, true, neuronDictionary);
            List<Neuron> isNeurons = scanForNewConnections(esNeurons, NType.IS, true, neuronDictionary);
            scanForNewConnections(isNeurons, NType.IS, false, neuronDictionary);
            scanForNewConnections(isNeurons, NType.ES, false, neuronDictionary);
            List<Neuron> emNeurons = scanForNewConnections(esNeurons, NType.EM, true, neuronDictionary);
            List<Neuron> imNeurons = scanForNewConnections(emNeurons, NType.IM, true, neuronDictionary);
            scanForNewConnections(isNeurons, NType.IM, false, neuronDictionary);
            scanForNewConnections(isNeurons, NType.EM, false, neuronDictionary);

            //remove neurons with no outgoing connections , it's quite a lot of work here, but gets payedback cause the simulation time is shorter than
            // finding circles is to expensive in this case
            List<Neuron> allNeurons = neuronDictionary.Values.ToList();


            foreach (Neuron n in allNeurons)
            {
                deleteRekursive(n);
            }

            List<Neuron> clearedNeuros = new List<Neuron>(allNeurons);
            int counter = 0;
            foreach (Neuron n in allNeurons)
            {
                if (!n.toDelete)
                {
                    n.index = counter++;
                    clearedNeuros.Add(n);
                }
            }
            //allNeurons.Where(n => n.toDelete == false).ToList();

            // build connection matrix  
            int matrixDim = clearedNeuros.Count;
            int[][] connectionsMat = new int[matrixDim][];
            for (int i = 0; i < matrixDim; i++)
            {
                connectionsMat[i] = new int[matrixDim];
            }
            NType[] typeArray = new NType[matrixDim];
            foreach (Neuron neuron in clearedNeuros)
            {
                typeArray[neuron.index] = neuron.type;
                foreach (Neuron outgoing in neuron.outCon)
                {
                    connectionsMat[neuron.index][outgoing.index] = 1;
                }
            }
            types = typeArray.ToList();
            return connectionsMat;
        }

        private void deleteRekursive(Neuron n)
        {

            if (n.type != NType.EM && n.outCon.Count == 0) // n.toDelete== false -> otherwise running in circle 
            {
                // always has a incoming connection, wouldn't be created otherwise
                Debug.Assert(n.toDelete == false); // there sould be no way to get to this one,cause it doesn't have outoing connections
                n.toDelete = true;
                foreach (Neuron n2 in n.inCon)
                {
                    Debug.Assert(n2.outCon.Remove(n));
                    deleteRekursive(n2);
                }
            }

        }

        private List<Neuron> scanForNewConnections(List<Neuron> sourceNeurons, NType targetType, bool createNewNeurons, Dictionary<PointF, Neuron> neuronDictionray) // ? make ntype Nullable
        {
            List<TempConnection> tempConnections = new List<TempConnection>();
            List<Neuron> destNeurons = new List<Neuron>();
            // Connections form Input nodes to ES nodes
            foreach (Neuron sourceNeuron in sourceNeurons)
            {
                // Analyze outgoing connectivity pattern from this input
                QuadPoint root = QuadTreeInitialisation(sourceNeuron.point.X, sourceNeuron.point.Y, true, (int)this.initialDepth, (int)this.maxDepth);
                tempConnections.Clear();
                // Traverse quadtree and add connections to list
                PruneAndExpress(sourceNeuron.point.X, sourceNeuron.point.Y, ref tempConnections, root, true, maxDepth);
                foreach (TempConnection p in tempConnections)
                {
                    PointF targetPoint = new PointF(p.x2, p.y2);
                    Neuron targetNeuron;
                    bool exists = neuronDictionray.TryGetValue(targetPoint, out targetNeuron);
                    if (exists && targetNeuron.type == targetType)
                    {
                        targetNeuron.inCon.Add(sourceNeuron);
                        sourceNeuron.outCon.Add(targetNeuron);
                    }
                    else if (createNewNeurons)
                    {
                        targetNeuron = new Neuron(targetType, targetPoint);
                        destNeurons.Add(targetNeuron);
                        neuronDictionray.Add(targetPoint, targetNeuron);
                        targetNeuron.inCon.Add(sourceNeuron);
                        sourceNeuron.outCon.Add(targetNeuron);
                    }

                }

            }
            return destNeurons;
        }





    }


}
