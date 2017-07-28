using System;
using System.Threading;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;


namespace SharpNeatLib.Experiments
{
    /// <summary>
    /// An implementation of IPopulationEvaluator that evaluates all new genomes(EvaluationCount==0)
    /// within the population using multiple threads, using an INetworkEvaluator provided at construction time.
    /// 
    /// This class provides an IPopulationEvaluator for use within the EvolutionAlgorithm by simply
    /// providing an INetworkEvaluator to its constructor. This usage is intended for experiments
    /// where the genomes are evaluated independently of each other (e.g. not simultaneoulsy in 
    /// a simulated world) using a fixed evaluation function that can be described by an INetworkEvaluator.
    /// </summary>
    public class MultiThreadedPopulationEvaluator : IPopulationEvaluator
    {
        public INetworkEvaluator networkEvaluator;
        protected IActivationFunction activationFn;

    
        protected static Semaphore sem = new Semaphore(HyperNEATParameters.numThreads, HyperNEATParameters.numThreads);
        protected static Semaphore sem2 = new Semaphore(1, 1);
        
       protected ulong evaluationCount = 0;

        #region Constructor

        public MultiThreadedPopulationEvaluator(INetworkEvaluator networkEvaluator, IActivationFunction activationFn)
        {
            this.networkEvaluator = networkEvaluator;
            this.activationFn = activationFn;
            ////@Max:  Minimize max threads nums to Cores
            //int o;
            //int w;
            //ThreadPool.GetMaxThreads(out w, out o);
            //ThreadPool.SetMaxThreads(3000, o);
            //int o2;
            //int w2;
            //     ThreadPool.GetMinThreads( out w2 , out o2);
            //ThreadPool.SetMinThreads(HyperNEATParameters.numThreads,o);
        }

        #endregion

        #region IPopulationEvaluator Members


        public void EvaluatePopulation(Population pop, EvolutionAlgorithm ea)
        {
   

            int count = pop.GenomeList.Count;
       
            evalPack e;
            IGenome g;
            int i;

            for (i = 0; i < count; i++)
            {


                sem.WaitOne();
                g = pop.GenomeList[i];
                e = new evalPack(networkEvaluator, activationFn, g,i);

                ThreadPool.QueueUserWorkItem(new WaitCallback(evalNet), e);

                // Update master evaluation counter.
                evaluationCount++;

            }
            
            //Erst aus methode raus wenn alle threads fertig sind
            for (int j = 0; j < HyperNEATParameters.numThreads; j++)
            {
                sem.WaitOne();
            }
            for (int j = 0; j < HyperNEATParameters.numThreads; j++)
            {
                sem.Release();
            }
        }



        public ulong EvaluationCount
        {
            get
            {
                return evaluationCount;
            }
        }

        public string EvaluatorStateMessage
        {
            get
            {	// Pass on the network evaluator's message.
                return networkEvaluator.EvaluatorStateMessage;
            }
        }

        public bool BestIsIntermediateChampion
        {
            get
            {	// Only relevant to incremental evolution experiments.
                return false;
            }
        }

        public bool SearchCompleted
        {
            get
            {	// This flag is not yet supported in the main search algorithm.
                return false;
            }
        }



        public static void evalNet(Object input)
        {

            evalPack e = (evalPack)input;

            if (e.g == null )//|| e.g.EvaluationCount != 0)
            {
                sem.Release();
                return;
            }
            sem2.WaitOne();
            INetwork network = e.g.Decode(e.Activation);
            sem2.Release();
            if (network == null)
            {	// Future genomes may not decode - handle the possibility.
                e.g.Fitness = EvolutionAlgorithm.MIN_GENOME_FITNESS;
            }
            else
            {
                DateTime dt = DateTime.Now;
                //sem2.WaitOne();
                //Console.WriteLine("starting withGenome Number {0}", e.nu);
                //sem2.Release();
                int numNeurons;
                e.g.Fitness = Math.Max(e.NetworkEvaluator.threadSafeEvaluateNetwork(network,sem2,out numNeurons), EvolutionAlgorithm.MIN_GENOME_FITNESS);
                sem2.WaitOne();
                Console.WriteLine("Genome Number {0} NumNeurons {1} fitness {2:0.#####} dt {3} ", e.nu,numNeurons,e.g.Fitness ,(DateTime.Now.Subtract(dt)));
                sem2.Release();
            }

            // Reset these genome level statistics.
            e.g.TotalFitness += e.g.Fitness;
            e.g.EvaluationCount += 1;
            sem.Release();

        }

        #endregion
    }

    class evalPack
    {

        INetworkEvaluator networkEvaluator;
        IActivationFunction activationFn;
        IGenome genome;
        int number;

        public evalPack(INetworkEvaluator n, IActivationFunction a, IGenome g, int nu)// number only for debug.. removeable!!
        {

            networkEvaluator = n;
            activationFn = a;
            genome = g;
            number = nu;
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
