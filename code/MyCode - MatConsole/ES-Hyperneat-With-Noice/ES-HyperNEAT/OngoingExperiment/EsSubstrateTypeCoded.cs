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
using static EsExperimentNS.EsSubstrate2D;

namespace EsExperimentNS
{

    public class EsSubstrateTypeCoded : EvolvableSubstrate
    {
        private int esIterations;

        public EsSubstrateTypeCoded(INetwork genome)
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

        // das cppn muss eine gewisse compelxität haben, damit hier modelle entstehen die d mit EM über ES neurone verknüpfen.
        // sollte sich dann im laufe der evolution zeigen.
        public int[][] getConnections(out List<NType> types)
        {
            uint numInputNeurons = ExperimentParameters.numDNeurons;
            // to HyperNeatParameter
           // int genomeIterations = HyperNEATParameters.ESIterations;

            Dictionary<PointF, Neuron> neuronDictionary = new Dictionary<PointF, Neuron>();


            List<Neuron[]> connections = new List<Neuron[]>(); //int 2 input output

            // Quadtree area : unit square with center 0
            //Place Input Neurons y= -0.5 x in area between -0.5 and 0.5
            List<Neuron> inputNeurons = new List<Neuron>();
            float inputDelta = 1f / (numInputNeurons - 1);
            float xPos = 0;
            float yPos = 0;
            for (int i = 0; i < numInputNeurons; i++)
            {
                PointF point = new PointF(xPos, yPos);
                Neuron n = new Neuron(NType.D, point);
                inputNeurons.Add(n);
                neuronDictionary.Add(point, n);
                xPos += inputDelta;
            }
            // need max depth of 3 = max 64+D neurons ,depth 4 = 256+D
            // neurons are stealing their space away. Give each neuron type a quarter of a s 2X2 square wth center 0, D cells are a line in the middle from 0 to one
            List<Neuron> lastNeurons = new List<Neuron>(inputNeurons);
            for (int i = 0; i < esIterations; i++)
            {
              List<Neuron> newNeuorns = scanForNewConnectionsAndPoints(inputNeurons, neuronDictionary );
                int countES = newNeuorns.Sum<Neuron>(n=> n.type == NType.ES? 1:0);
               int countEM = newNeuorns.Sum<Neuron>(n => n.type == NType.EM ? 1 : 0);
                int countIM = newNeuorns.Sum<Neuron>(n => n.type == NType.IM ? 1 : 0);
               int countIS = newNeuorns.Sum<Neuron>(n => n.type == NType.IS ? 1 : 0);


                lastNeurons = newNeuorns;
              if (lastNeurons.Count == 0) break;
               // no search from old neurons to new ones necessary, cause if there would be a connection the neuron owuld have been initialized by this former neuron.
            }
            // last new neurons, connections have to be searched, but no new points
            scanForNewConnectionsAndPoints(lastNeurons, neuronDictionary,false);


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
                        n.index =counter++;
                        sortedNeurons.Add(n);
                    }
                }

            }
            Debug.Assert(counter == clearedNeuros.Count );

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
            return connectionsMat;// hier fliegt macnhmal ein unnachvollziehbarar Argument Null Exception
        }



        private void deleteNodesRekursive(Neuron n)
        {

            if (n.type != NType.EM && n.outCon.Count == 0 && n.toDelete == false) // n.toDelete== false -> otherwise running in circle 
            {
                //Debug.Assert(n.toDelete == false); // there sould be no way to get to this ,cause it doesn't have outgoing connections
                                                       // no there is a way cause one iterates over all neurons revursive deleted neurons are called again
                // TODO assert schlägt fehl
                n.toDelete = true;
                foreach (Neuron n2 in n.inCon)
                {
                    Debug.Assert(n2.outCon.Remove(n));
                    deleteNodesRekursive(n2);
                }
                n.inCon.Clear();
            }

        }

        private List<Neuron> scanForNewConnectionsAndPoints(List<Neuron> sourceNeurons , Dictionary<PointF, Neuron> neuronDictionray, bool createNewNeurons = true) 
        {
           // int connection_counter = 0;
            List<TempConnection> tempConnections = new List<TempConnection>();
            List<Neuron> destNeurons = new List<Neuron>();
            // Connections form Input nodes to ES nodes
            foreach (Neuron sourceNeuron in sourceNeurons)
            {
                // Analyze outgoing connectivity pattern from this input
                QuadPoint root = QuadTreeInitialisation(sourceNeuron.point.X, sourceNeuron.point.Y, true, (int)this.initialDepth,(int) this.maxDepth);
                tempConnections.Clear();
                // Traverse quadtree and add connections to list
                PruneAndExpress(sourceNeuron.point.X, sourceNeuron.point.Y, ref tempConnections, root, true, maxDepth);
                foreach (TempConnection p in tempConnections)
                {
                    PointF targetPoint = new PointF(p.x2, p.y2);
                    Neuron targetNeuron;
                    bool exists = neuronDictionray.TryGetValue(targetPoint, out targetNeuron);
                    if (exists && isConnectable(sourceNeuron.type,targetNeuron.type))
                    {
                    //    connection_counter++;
                        targetNeuron.inCon.Add(sourceNeuron);
                        sourceNeuron.outCon.Add(targetNeuron);
                    }
                    else if (!exists && createNewNeurons )
                    { // neuron is always created even if it is not connectable, otherwise connections are traced iterative like in Essbustrate
                        NType targetType = getNeuronType(targetPoint);
                        targetNeuron = new Neuron(targetType, targetPoint);
                        destNeurons.Add(targetNeuron);
                        neuronDictionray.Add(targetPoint, targetNeuron);
                        if (isConnectable(sourceNeuron.type, targetType))
                        {
                            targetNeuron.inCon.Add(sourceNeuron);
                            sourceNeuron.outCon.Add(targetNeuron);
                        }
                    }

                }

            }
            return destNeurons;
        }

        private NType getNeuronType(PointF target)
        {
            float[] coordinates = new float[numCPPNInputs];
            float weightRange = (float)HyperNEATParameters.weightRange;
            coordinates[4] = target.X;
            coordinates[5] = target.Y;
            int iterations = 2 * (genome.TotalNeuronCount - (genome.InputNeuronCount + genome.OutputNeuronCount)) + 1;

            genome.ClearSignals();
            genome.SetInputSignals(coordinates);
            genome.MultipleSteps(iterations);
            bool output1 = genome.GetOutputSignal(1) >0.0 ; //output function bipolar sigmoid [-1,1]
            bool output2 = genome.GetOutputSignal(2) >0.0 ;
            // 00=IS, 01= IM, 10=ES, 11 EM;
            if (output1)
            {
                if (output2) return NType.EM;
                else return NType.ES;
            }
            else 
            {
                if (output2) return NType.IM;
                else return NType.IS;
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
