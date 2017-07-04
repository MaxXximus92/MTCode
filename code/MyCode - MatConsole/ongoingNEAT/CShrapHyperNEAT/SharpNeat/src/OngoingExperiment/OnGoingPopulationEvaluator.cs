using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Evolution;
using System.Threading;

namespace OngoingExperimentNS
{
   public class OnGoingPopulationEvaluator : MultiThreadedPopulationEvaluator
    {

        public OnGoingPopulationEvaluator(INetworkEvaluator eval)
            : base(eval, null)
        {

        }


        public void evaluateAndMatlabPlot(IGenome[] g, string[] names, string namePrefix)
        {
            for (int i = 0; i < g.Length; i++)
            {


                sem.WaitOne();
                evalPack2 e = new evalPack2(networkEvaluator, activationFn, g[i], i, namePrefix+names[i]);

                ThreadPool.QueueUserWorkItem(new WaitCallback(plotNet), e);

                // Update master evaluation counter.
                evaluationCount++;


            }
            for (int j = 0; j < HyperNEATParameters.numThreads; j++)
            {
                sem.WaitOne();
            }
            for (int j = 0; j < HyperNEATParameters.numThreads; j++)
            {
                sem.Release();
            }

        }

        public static void  plotNet(Object input)
        {
            evalPack2 e = (evalPack2)input;

            if (e.g == null)//|| e.g.EvaluationCount != 0)
            {
                sem.Release();
                return;
            }
            sem2.WaitOne();
            INetwork network = e.g.Decode(e.Activation);
            sem2.Release();
            DateTime dt = DateTime.Now;
            e.g.Fitness = ((OnGoingNetworkEvaluator)(e.NetworkEvaluator)).plotMatlabGraphsAndSafeNetwork(network, e.Name);
            Console.WriteLine("Matlab Plots for Genome Name " + e.Name + " created " + (DateTime.Now.Subtract(dt)));

            sem.Release();
        }
    }

    public class evalPack2
    {

        INetworkEvaluator networkEvaluator;
        IActivationFunction activationFn;
        IGenome genome;
        int number;
        string name;

        public evalPack2(INetworkEvaluator n, IActivationFunction a, IGenome g, int nu, string name)// number only for debug.. removeable!!
        {

            networkEvaluator = n;
            activationFn = a;
            genome = g;
            number = nu;
            this.name = name;
        }
        public String Name
        {
            get{
                return name;
            }
        }

        public INetworkEvaluator NetworkEvaluator
        {
            get
            {
                return networkEvaluator;
            }
        }

        public IActivationFunction Activation
        {
            get
            {
                return activationFn;
            }
        }

        public IGenome g
        {
            get
            {
                return genome;
            }
        }

        public int nu
        {
            get
            {
                return number;
            }
        }

    }
}
