using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using System.Collections.Concurrent;

namespace OngoingExperimentNS
{
      public class OnGoingNetworkEvaluator : INetworkEvaluator
    {
        //public static SkirmishSubstrate substrate;
        private int threadCounter = 0;
        private readonly ConcurrentDictionary<int, MatlabCommunicator> dict = new ConcurrentDictionary<int, MatlabCommunicator>();
        private readonly int[] numInsOuts;
        public OnGoingNetworkEvaluator()
        {
            MatlabCommunicator matlab = new MatlabCommunicator(ExperimentParameters.matlabPath); //TODO generating new connection always -> very expensive , reuse connection in some way.
            matlab.generateModel(ExperimentParameters.numNeurons, ExperimentParameters.spikingThreshold, ExperimentParameters.matlabSavePath);
            numInsOuts = matlab.getEsEmNum();
            //substrate = new SkirmishSubstrate(5 * agents, 3 * agents, 5 * agents, HyperNEATParameters.substrateActivationFunction);
        }
        public string EvaluatorStateMessage => throw new NotImplementedException();

        public double EvaluateNetwork(INetwork network)
        {
            throw new NotImplementedException();
        }

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem)
        {
            MatlabCommunicator matlab;
           
            int id = Thread.CurrentThread.ManagedThreadId; // anscheinend ist sowohl hashcode als auch id nicht eindeutig genug um kollisionen zu vermeiden -> set unique name
            if (dict.ContainsKey(id))   
            {
                matlab = dict[id];
            }
            else
            {
                matlab = new MatlabCommunicator(ExperimentParameters.matlabPath); 
                matlab.generateModel(ExperimentParameters.numNeurons, ExperimentParameters.spikingThreshold, ExperimentParameters.matlabSavePath); 
               // sem.WaitOne();
                dict.TryAdd(id, matlab);
               // sem.Release();
                Console.WriteLine("matlab instance for thread added: "+ id +" thread number"+ ++threadCounter);
 
            }
            
            double[,] weights = OngoingSubstrate.getWeights(network, numInsOuts[0], numInsOuts[1]);
            matlab.setEsEmWeightsMatrix(weights);
            // fitness negative cause higher values should be more fitt
            double fitness =  matlab.startSimulation(ExperimentParameters.runTime, ExperimentParameters.runSettings,ExperimentParameters.trailsPerSetting);
            fitness = -fitness + 180; // max rmsd 180 -> scale fitness to be positiv. Error of 180 is fitness 0; max fitness = 180
            matlab.resetModel(); ;
            return fitness;

            // 0.02 sekunden da nur 4(sind irgendwie ein paar mehr threads ca 8 ) verbidnungen benötigt

        }
        
        public void plotMatlabGraphsAndSafeNetwork(INetwork network, string saveName)
        {

            MatlabCommunicator matlab = new MatlabCommunicator(ExperimentParameters.matlabPath); //TODO generating new connection always -> very expensive , reuse connection in some way.
            matlab.generateModel(ExperimentParameters.numNeurons, ExperimentParameters.spikingThreshold,ExperimentParameters.matlabSavePath); //TODO might be possible to this only once but likely problems with coherency
            int[] numInsOuts = matlab.getEsEmNum();
            double[,] weights = OngoingSubstrate.getWeights(network, numInsOuts[0], numInsOuts[1]);
            matlab.setEsEmWeightsMatrix(weights);
            // fitness negative cause higher values should be more fitt
            double fitness = -matlab.startSimulation(ExperimentParameters.runTime, ExperimentParameters.runSettings, ExperimentParameters.trailsPerSetting,true);
            fitness = -fitness + 180;
            matlab.saveModel(saveName);
            matlab.close();
            // verbindungsaufbau und model generierung dauert 0.25 sekunden -> fällt also nicht ins gewicht da modelevaluation bei 1200 schritten schon bei 2.0 liegt
        }

    }
}
