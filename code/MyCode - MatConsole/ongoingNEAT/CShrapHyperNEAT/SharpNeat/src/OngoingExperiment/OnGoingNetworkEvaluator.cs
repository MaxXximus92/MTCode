using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using System.Collections.Concurrent;
using static OngoingExperimentNS.MatlabCommunicator;

namespace OngoingExperimentNS
{
    public class OnGoingNetworkEvaluator : INetworkEvaluator
    {
        //public static SkirmishSubstrate substrate;
        private int mlInstanceCounter = 0;

        private readonly List<MatlabCommunicator> matlabInstances = new List<MatlabCommunicator>();
        private readonly Semaphore sema = new Semaphore(1, 1);
        private readonly int[] numInsOuts;

        //  private readonly Semaphore sema = new Semaphore(1, 1);




        public OnGoingNetworkEvaluator(int[] esEmNums)
        {
            numInsOuts = esEmNums;

        }

        public string EvaluatorStateMessage => throw new NotImplementedException();

        public double EvaluateNetwork(INetwork network)
        {
            throw new NotImplementedException();
        }

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem)
        {


            MatlabCommunicator matlab = getMatlab();
            double fitness = 0;
            double[][] esEmweights = OngoingSubstrate.getConnections(network, numInsOuts[0], numInsOuts[1]);
            try
            {
                 fitness = matlab.simulate(esEmweights);
            }
            catch(MatlabNotRespondingException e)
            {
                Console.WriteLine(e.Message);
                handleCrashedMatlab(matlab);
                return threadSafeEvaluateNetwork(network,sem);
            }
            fitness = -fitness + 135; // max rmsd 180 -> scale fitness to be positiv. Error of 180 is fitness 0; max fitness = 180
            return fitness;

            // 0.02 sekunden da nur 4(sind irgendwie ein paar mehr threads ca 8 ) verbidnungen benötigt

        }

        public double plotMatlabGraphsAndSafeNetwork(INetwork network, string saveName)
        {
            double fitness = 0;
            MatlabCommunicator matlab = getMatlab();
            double[][] esEmweights = OngoingSubstrate.getConnections(network, numInsOuts[0], numInsOuts[1]);
            try
            {
                 fitness = matlab.simulateWithPlot(esEmweights, saveName);
            }
            catch (MatlabNotRespondingException e)
            {
                Console.WriteLine(e.Message);
                handleCrashedMatlab(matlab);
                plotMatlabGraphsAndSafeNetwork(network, saveName);
            }
            fitness = -fitness + 135; // max rmsd 180 -> scale fitness to be positiv. Error of 180 is fitness 0; max fitness = 180
            return fitness;
        }

        private void handleCrashedMatlab(MatlabCommunicator matlab)
        {
            sema.WaitOne();
            int counter = ++mlInstanceCounter;
            string name = "com_" + mlInstanceCounter;
            sema.Release();
            MatlabCommunicator newMatlab = new MatlabCommunicator(name);
            sema.WaitOne();
            matlabInstances.Remove(matlab);
            Console.WriteLine("Matlab instance {0} removed and closed", matlab.name);
            matlabInstances.Add(newMatlab);
            Console.WriteLine("Matlab instance Number {0} added", counter);
            sema.Release();
        }

        public void closeMatlabInstances()
        {
            sema.WaitOne();

            foreach (MatlabCommunicator m in matlabInstances)
            {
                m.close();
            }
            matlabInstances.Clear();
            sema.Release();
        }

        private MatlabCommunicator getMatlab()
        {

            sema.WaitOne();
            foreach (MatlabCommunicator m in matlabInstances) 
            {
                if (m.block())
                {
                   // Console.WriteLine("Free Matlab Instance "+ m.name +" was taken");
                    sema.Release();
                    return m;
                }
            }
            sema.Release();

            sema.WaitOne();
            mlInstanceCounter++;
            string name = "com_" + mlInstanceCounter;
            sema.Release();

            MatlabCommunicator matlab = new MatlabCommunicator(name);

            sema.WaitOne();
            matlab.block();
            matlabInstances.Add(matlab);
            Console.WriteLine("Matlab instance Number {0} added", matlabInstances.Count);
            sema.Release();

            return matlab;


        }
    }
}
