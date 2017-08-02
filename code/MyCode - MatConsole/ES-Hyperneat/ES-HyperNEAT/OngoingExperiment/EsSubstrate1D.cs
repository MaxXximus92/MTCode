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
using static EsExperimentNS.EsSubstrate;

namespace EsExperimentNS
{

    public class EsSubstrate1D : EvolvableSubstrate1D
    {
        private int esIterations;

        public EsSubstrate1D(INetwork genome)
        {
            this.varianceThreshold = (float)HyperNEATParameters.varianceThreshold;
            this.bandThreshold = (float)HyperNEATParameters.bandingThreshold;
            this.divisionThreshold = (float)HyperNEATParameters.divisionThreshold;
            this.maxDepth = HyperNEATParameters.maximumDepth;
            this.genome = genome;
            this.initialDepth = HyperNEATParameters.initialDepth;
            this.esIterations = HyperNEATParameters.ESIterations;
            this.numCPPNInputs = (int)ExperimentParameters.cPPNInputs;

        }


        internal class Neuron
        {
            public NType type;
            public float pos;
            public List<Neuron> outCon = new List<Neuron>();
            public List<Neuron> inCon = new List<Neuron>();
            public bool toDelete = false;
            public int index = -1;
            public Neuron(NType type, float pos)
            {
                this.type = type;
                this.pos = pos;
            }


        }

        // das cppn muss eine gewisse compelxität haben, damit hier modelle entstehen die d mit EM über ES neurone verknüpfen.
        // sollte sich dann im laufe der evolution zeigen.
        public int[][] getConnections(out List<NType> types)
        {


            Dictionary<float, Neuron> neuronDictionary = new Dictionary<float, Neuron>();


            List<Neuron[]> connections = new List<Neuron[]>(); //int 2 input output

            // Quadtree area : 2x2 square with center 0
            

            
         // first numInputNeurons found source neurons are D neurons
            // need max depth of 3 = max 64+D neurons ,depth 4 = 256+D
  
                 scanForConnectionsAndPoints(neuronDictionary);
            //Debugging
            List<Neuron> neurons = neuronDictionary.Values.ToList();
                int countD = neurons.Sum<Neuron>(n => n.type == NType.D ? 1 : 0);
                int countES = neurons.Sum<Neuron>(n => n.type == NType.ES ? 1 : 0);
                int countEM = neurons.Sum<Neuron>(n => n.type == NType.EM ? 1 : 0);
                int countIM = neurons.Sum<Neuron>(n => n.type == NType.IM ? 1 : 0);
                int countIS = neurons.Sum<Neuron>(n => n.type == NType.IS ? 1 : 0);



            List<Neuron> allNeurons = neuronDictionary.Values.ToList();

            // TODO important  comment int !! after testin
            //remove neurons with no outgoing connections , it's quite a lot of work here, but gets payedback cause the simulation time is shorter than
            // finding circles is to expensive in this case

            foreach (Neuron n in allNeurons)
            {
                deleteNodesRekursive(n);
            }

            List<Neuron> clearedNeuros = new List<Neuron>();

            //catch all neurons not to delete

            foreach (Neuron n in allNeurons)
            {
                if (!n.toDelete)
                {
                    clearedNeuros.Add(n);
                }
            }
            // sort neurons by type
            int counter = 0;
            List<Neuron> sortedNeurons = new List<Neuron>();
            foreach (NType type in Enum.GetValues(typeof(NType)))
            {
                foreach (Neuron n in clearedNeuros)
                {
                    if (n.type == type)
                    {
                        n.index = counter++;
                        sortedNeurons.Add(n);
                    }
                }

            }
            Debug.Assert(counter == clearedNeuros.Count);

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
                    if (neuron.index != -1 && outgoing.index != -1)
                    {//Fehler !outgoing connection index kann -1 sein, ka wo der herkommt, sollte nicht passieren. so ist es erstmal gefixt
                        connectionsMat[neuron.index][outgoing.index] = 1;
                    }
                }
            }
            types = typeArray.ToList();
            return connectionsMat;// hier fliegt manchmal eine unnachvollziehbarar Argument Null Exception
        }



        private void deleteNodesRekursive(Neuron n)
        {

            if (n.type != NType.EM && n.outCon.Count == 0 && n.toDelete == false) // n.toDelete== false -> otherwise running in circle 
            {
                //Debug.Assert(n.toDelete == false); // there sould be no way to get to this ,cause it doesn't have outgoing connections
                // no there is a way cause one iterates over all neurons revursive deleted neurons are called again
                n.toDelete = true;
                foreach (Neuron n2 in n.inCon)
                {
                    Debug.Assert(n2.outCon.Remove(n));
                    deleteNodesRekursive(n2);
                }
                n.inCon.Clear();
            }

        }

        private void scanForConnectionsAndPoints( Dictionary<float, Neuron> neuronDictionray)
        {
            // int connection_counter = 0;
            List<TempConnection> tempConnections = new List<TempConnection>();
            QuadPoint root = QuadTreeInitialisation( (int)this.initialDepth, (int)this.maxDepth);
                // Traverse quadtree and add connections to list
                PruneAndExpress( ref tempConnections, root, maxDepth);
                foreach (TempConnection p in tempConnections)
                {
                    float sourcePos = p.x1;
                    float targetPos = p.x2;

                Neuron sourceNeuron;
                if (!neuronDictionray.TryGetValue(sourcePos, out sourceNeuron))
                {
                    NType  type = getNeuronType(sourcePos); 
                    sourceNeuron = new Neuron(type, sourcePos);

                    neuronDictionray.Add(sourcePos, sourceNeuron);

                }

                Neuron targetNeuron;
                if (!neuronDictionray.TryGetValue(targetPos, out targetNeuron))
                {
                    targetNeuron = new Neuron(getNeuronType(targetPos), targetPos);

                    neuronDictionray.Add(targetPos, targetNeuron);
                }
          
                if ( isConnectable(sourceNeuron.type, targetNeuron.type))
                {
                    //    connection_counter++;
                    targetNeuron.inCon.Add(sourceNeuron);
                    sourceNeuron.outCon.Add(targetNeuron);
                }
                    

                }
        }

        private NType getNeuronType(float target)
        {
            float[] coordinates = new float[numCPPNInputs];
            float weightRange = (float)HyperNEATParameters.weightRange;
            coordinates[2] = target;
            int iterations = 2 * (genome.TotalNeuronCount - (genome.InputNeuronCount + genome.OutputNeuronCount)) + 1;

            genome.ClearSignals();
            genome.SetInputSignals(coordinates);
            genome.MultipleSteps(iterations);
            bool output1 = genome.GetOutputSignal(1) > 0.0;
            bool output2 = genome.GetOutputSignal(2) > 0.0;
            bool output3 = genome.GetOutputSignal(3) > 0.0;
            // 000 D 
            // 001 D
            // 010 EM
            // 011 EM
            // 100 ES
            // 101 ES
            // 110 IM
            // 111 IS
            if (output1) //1
            {
                if (output2) // 11
                {
                    if (output3) return NType.IS; // 111
                    else return NType.IM; // 110
                }
                else //101 u 100
                {
                    return NType.ES;
                }
            }
            else // 0 
            {
                if (output2) // 010 011
                {
                    return NType.EM;
                }
                else //001 u 000
                {
                    return NType.D;
                }
            }

        }

        private bool isConnectable(NType source, NType target)
        {
            switch (source)
            {
                case NType.EM:
                    if (target == NType.IM) return true;
                    return false;
                case NType.IM:
                    if (target == NType.EM || target == NType.IM) return true;
                    return false;
                case NType.ES:
                    if (target == NType.EM || target == NType.IS) return true;
                    return false;
                case NType.IS:
                    if (target == NType.ES || target == NType.IS) return true;
                    return false;
                case NType.D:
                    if (target == NType.ES) return true;
                    return false;

            }
            return false;
        }





    }


}
