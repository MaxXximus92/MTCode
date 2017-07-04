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
    
       
        private readonly int[] numInsOuts;
        private readonly double[,] netWeights;
        private readonly double[,] netEqParams;
      //  private readonly Semaphore sema = new Semaphore(1, 1);




        public OnGoingNetworkEvaluator(double[,] netWeights, double[,] equationParams, int[] esEmNums )
        {
            this.netWeights = netWeights;
            this.netEqParams = equationParams;
            numInsOuts = esEmNums;

        }

        public string EvaluatorStateMessage => throw new NotImplementedException();

        public double EvaluateNetwork(INetwork network)
        {
            throw new NotImplementedException();
        }

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem)
        {
            OngoingModelWrapper  model = new OngoingModelWrapper();
            
            double[,] esEmweights = OngoingSubstrate.getConnections(network, numInsOuts[0], numInsOuts[1]);
            double fitness = model.runModel("",(int)ExperimentParameters.numNeurons,(int)ExperimentParameters.runTime,ExperimentParameters.runSettings,ExperimentParameters.matlabSavePath, netEqParams, netWeights,esEmweights,false);
            fitness = -fitness + 180; // max rmsd 180 -> scale fitness to be positiv. Error of 180 is fitness 0; max fitness = 180
            return fitness;

            // 0.02 sekunden da nur 4(sind irgendwie ein paar mehr threads ca 8 ) verbidnungen benötigt

        }
        
        public void plotMatlabGraphsAndSafeNetwork(INetwork network, string saveName)
        {

            OngoingModelWrapper model = new OngoingModelWrapper();
            double[,] esEmweights = OngoingSubstrate.getConnections(network, numInsOuts[0], numInsOuts[1]);
            double fitness = model.runModel(saveName, (int)ExperimentParameters.numNeurons, (int)ExperimentParameters.runTime, ExperimentParameters.runSettings, ExperimentParameters.matlabSavePath, netEqParams, netWeights, esEmweights, true);
           
        }

   

    }
}
