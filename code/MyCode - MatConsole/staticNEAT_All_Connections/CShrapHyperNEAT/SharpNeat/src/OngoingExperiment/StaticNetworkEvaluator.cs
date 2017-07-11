﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using System.Collections.Concurrent;
using static StaticExperimentNS.MatlabCommunicator;

namespace StaticExperimentNS
{
    public class StaticNetworkEvaluator : INetworkEvaluator
    {
        //public static SkirmishSubstrate substrate;
        private int mlInstanceCounter = 0;

        private readonly List<MatlabCommunicator> matlabInstances = new List<MatlabCommunicator>();
        private readonly Semaphore sema = new Semaphore(1, 1);
        private readonly ModelNeuronType[] neuronTypes;

        //  private readonly Semaphore sema = new Semaphore(1, 1);




        public StaticNetworkEvaluator(ModelNeuronType[] neuronTypes)
        {
            this.neuronTypes = neuronTypes;

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
            double[][] connections = StaticSubstrate.getConnectionsSquareModel(network, neuronTypes);
            try
            {
                fitness = matlab.simulate(connections);
            }
            catch (MatlabCrashedException e)
            {
                Console.WriteLine(e.Message);
                handleCrashedMatlab(matlab);
                return threadSafeEvaluateNetwork(network, sem);
            }
            
            return fitness;

           

        }

        public void plotMatlabGraphsAndSafeNetwork(INetwork network, string saveName)
        {

            MatlabCommunicator matlab = getMatlab();
            double[][] connections = StaticSubstrate.getConnectionsSquareModel(network,neuronTypes);
            try
            {
                double fitness = matlab.simulateWithPlot(connections, saveName);
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
                double fitness = MatlabCommunicator.startRunModelSimulate( saveName);
            }
            catch (MatlabCrashedException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("simulating model again");
                simulateModel( saveName);
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
            Console.WriteLine("Matlab instance {0} removed ", matlab.name);
            matlabInstances.Add(newMatlab);
            Console.WriteLine("Matlab instance Number {0} added", counter);
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
            Console.WriteLine("Matlab instance Number {0} added", matlabInstances.Count);
            sema.Release();

            return matlab;


        }
    }
}
