using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using System.Collections.Concurrent;
using static EsExperimentNS.MatlabCommunicator;
using static EsExperimentNS.EsSubstrate2D;

namespace EsExperimentNS
{
    public class EsNetworkEvaluator : INetworkEvaluator
    {
 
        private int mlInstanceCounter = 0;

        private readonly List<MatlabCommunicator> matlabInstances = new List<MatlabCommunicator>();
        private readonly Semaphore sema = new Semaphore(1, 1);


      




        public EsNetworkEvaluator()
        {


        }


        public string EvaluatorStateMessage => throw new NotImplementedException();

        public double EvaluateNetwork(INetwork network)
        {
            throw new NotImplementedException();
        }

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem, out int numNeurons)
        {

            double fitness = 0;
            EsSubstrate2D substrate = new EsSubstrate2D(network);
           // EsSubstrateTypeCoded substrate = new EsSubstrateTypeCoded(network);
           // EsSubstrate1D substrate = new EsSubstrate1D(network);
            List<NType> types = new List<NType>();
            int[][] connections = substrate.getConnections(out types);
            numNeurons = types.Count;
            if (connections.Length == 0 || types.Count == 0)
                return 0;
            MatlabCommunicator matlab = getMatlab();
            try
            {
                fitness = matlab.simulate(connections,types);
            }
            catch (MatlabCrashedException e)
            {
                Console.WriteLine(e.Message);
                handleCrashedMatlab(matlab);
                return threadSafeEvaluateNetwork(network, sem,out numNeurons);
            }
            
            return fitness;

           

        }

        public void plotMatlabGraphsAndSafeNetwork(INetwork network, string saveName)
        {


             EsSubstrate2D substrate = new EsSubstrate2D(network);
           // EsSubstrateTypeCoded substrate = new EsSubstrateTypeCoded(network);
           // EsSubstrate1D substrate = new EsSubstrate1D(network);
            List<NType> types = new List<NType>();
            int[][] connections = substrate.getConnections(out types);
            if (connections.Length == 0 || types.Count == 0)
                return;
            MatlabCommunicator matlab = getMatlab();
            try
            {
                double fitness = matlab.simulateWithPlot(connections,types, saveName);
            }
            catch (MatlabCrashedException e)
            {
                Console.WriteLine(e.Message);
                handleCrashedMatlab(matlab);
                plotMatlabGraphsAndSafeNetwork(network, saveName);
            }
        }

        public void simulateModel( string saveName)
        {
            try
            {
                double fitness = MatlabCommunicator.startRunModelSimulate(saveName);
            }
            catch (MatlabCrashedException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("simulating model again");
                simulateModel(saveName);
            }
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

        private void handleCrashedMatlab(MatlabCommunicator matlab)
        {
            sema.WaitOne();
            int counter = ++mlInstanceCounter;
            string name = "com_" + mlInstanceCounter;
            sema.Release();
            MatlabCommunicator newMatlab = new MatlabCommunicator(name);
            sema.WaitOne();
            matlabInstances.Remove(matlab);
            Console.WriteLine("matlab instance {0} removed ", matlab.name);
            matlabInstances.Add(newMatlab);
            Console.WriteLine("matlab instance number {0} added", counter);
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
            mlInstanceCounter++;
            string name = "com_" + mlInstanceCounter;
            sema.Release();

            MatlabCommunicator matlab = new MatlabCommunicator(name);

            sema.WaitOne();
            matlab.block();
            matlabInstances.Add(matlab);
            Console.WriteLine("matlab instance number {0} added", matlabInstances.Count);
            sema.Release();

            return matlab;


        }
    }
}
