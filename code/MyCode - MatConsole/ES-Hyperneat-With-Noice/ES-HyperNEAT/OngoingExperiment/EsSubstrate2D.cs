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

    public class EsSubstrate2D : EvolvableSubstrate
    {
        public EsSubstrate2D(INetwork genome)
        {
            this.varianceThreshold = (float)HyperNEATParameters.varianceThreshold;
            this.bandThreshold = (float)HyperNEATParameters.bandingThreshold;
            this.divisionThreshold = (float)HyperNEATParameters.divisionThreshold;
            this.maxDepth = HyperNEATParameters.maximumDepth;
            this.genome = genome;
            this.initialDepth = HyperNEATParameters.initialDepth;
            this.numCPPNInputs =(int) ExperimentParameters.cPPNInputs;

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
            List<Neuron> esNeurons = scanForNewConnectionsAndPoints(inputNeurons, NType.ES, true, neuronDictionary,new PointF(0.5f,0.5f),1f,(int)this.maxDepth);
            List<Neuron> isNeurons = scanForNewConnectionsAndPoints(esNeurons, NType.IS, true, neuronDictionary, new PointF(-0.5f, 0.5f), 1f, (int)this.maxDepth);
            scanForNewConnections(isNeurons, NType.IS, neuronDictionary);
            scanForNewConnections(isNeurons, NType.ES, neuronDictionary);
            List<Neuron> emNeurons = scanForNewConnectionsAndPoints(esNeurons, NType.EM, true, neuronDictionary, new PointF(-0.5f, -0.5f), 1f, (int)this.maxDepth);
            List<Neuron> imNeurons = scanForNewConnectionsAndPoints(emNeurons, NType.IM, true, neuronDictionary, new PointF(0.5f, -0.5f), 1f, (int)this.maxDepth);
            scanForNewConnections(isNeurons, NType.IM, neuronDictionary);
            scanForNewConnections(isNeurons, NType.EM, neuronDictionary);

            //remove neurons with no outgoing connections , it's quite a lot of work here, but gets payedback cause the simulation time is shorter than
            // finding circles is to expensive in this case
            List<Neuron> allNeurons = neuronDictionary.Values.ToList();

          

            foreach (Neuron n in allNeurons)
            {
                deleteRekursive(n);
            }

            List<Neuron> clearedNeuros = new List<Neuron>();

         
            //get not deleted neurons

            foreach (Neuron n in allNeurons)
            {
                if (!n.toDelete)
                {
                    clearedNeuros.Add(n);
                }
            }
            // sort neurons by type
  
            int counter = 0;
            foreach (NType type in Enum.GetValues(typeof(NType)))
            {
                foreach (Neuron n in clearedNeuros)
                {
                    if (n.type == type)
                    {
                        n.index =counter++;
                    }
                }

            }
            Debug.Assert(counter == clearedNeuros.Count );
           // Console.WriteLine("Here 5");
            //allNeurons.Where(n => n.toDelete == false).ToList();

            // build connection matrix  
            int matrixDim = clearedNeuros.Count;
   
            int[][] connectionsMat = new int[matrixDim][];
            for (int i = 0; i < matrixDim; i++)
            {
                connectionsMat[i] = new int[matrixDim];
            }
           // Console.WriteLine("Here 6");
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
         //   Console.WriteLine("Here 7");
            types = typeArray.ToList();
            return connectionsMat;// hier fliegt macnhmal ein unnachvollziehbarar Argument Null Exception
        }
  
        private void deleteRekursive(Neuron n)
        {

            if (n.type != NType.EM && n.outCon.Count == 0 && n.toDelete == false) // n.toDelete== false -> otherwise running in circle 
            {
                //Debug.Assert(n.toDelete == false); // there sould be no way to get to this ,cause it doesn't have outgoing connections
                                                       // no there is a way cause one iterates over all neurons revursive deleted neurons are called again

                n.toDelete = true;
                foreach (Neuron n2 in n.inCon)
                {
                    // Debug.Assert(n2.outCon.Remove(n));
                    if (!n2.outCon.Remove(n)) throw new Exception("target incoming was not source outgoing connection");
                    deleteRekursive(n2);
                }
                n.inCon.Clear();
            }

        }

       private void  scanForNewConnections(List<Neuron> sourceNeurons, NType targetType, Dictionary<PointF, Neuron> neuronDictionray)
        {
            scanForNewConnectionsAndPoints(sourceNeurons,  targetType,false, neuronDictionray, new PointF(0,0),2, (int)this.maxDepth+1);
        }
        private List<Neuron> scanForNewConnectionsAndPoints(List<Neuron> sourceNeurons, NType targetType, bool createNewNeurons, Dictionary<PointF, Neuron> neuronDictionray, PointF center, float width,int maximalDepth) 
        {
            
            List<TempConnection> tempConnections = new List<TempConnection>();
            List<Neuron> destNeurons = new List<Neuron>();
            // Connections form Input nodes to ES nodes
            foreach (Neuron sourceNeuron in sourceNeurons)
            {
                // Analyze outgoing connectivity pattern from this input
                QuadPoint root = QuadTreeInitialisation(sourceNeuron.point.X, sourceNeuron.point.Y, true, (int)this.initialDepth, maximalDepth, center.X,center.Y,width);
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
                    else if (!exists && createNewNeurons)
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
