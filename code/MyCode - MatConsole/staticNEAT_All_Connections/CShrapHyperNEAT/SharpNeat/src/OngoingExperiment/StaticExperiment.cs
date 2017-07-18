using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using static StaticExperimentNS.MatlabCommunicator;

namespace StaticExperimentNS
{
    public class StaticExperiment : IExperiment
    {
        int cPPNinputs;
        int cPPNoutputs;
        IPopulationEvaluator populationEvaluator = null;
        NeatParameters neatParams = null;
        ModelNeuronType[] neuronTypes;

        public StaticExperiment(int cPPNinputs, int cPPNoutputs, ModelNeuronType[] neuronTypes)
        {
            this.cPPNinputs = cPPNinputs;
            this.cPPNoutputs = cPPNoutputs;
            this.neuronTypes = neuronTypes;

        }
        public IPopulationEvaluator PopulationEvaluator
        {
            get
            {
                if (populationEvaluator == null)
                    ResetEvaluator(HyperNEATParameters.substrateActivationFunction); 

                return populationEvaluator;
            }
        }

        public int InputNeuronCount
        {
            get
            {
                return cPPNinputs;
            }
        }

        public int OutputNeuronCount
        {
            get
            {
                return cPPNoutputs;
            }
        }

        public NeatParameters DefaultNeatParameters
        {
            get
            {
                if (neatParams == null)
                {
                    NeatParameters np = new NeatParameters();
                    np.connectionWeightRange = 3;
                    np.pMutateAddConnection = .15; // changed 1D case before 0.03
                    np.pMutateAddNode = .05; // changed 1d Case   before 0.01
                    np.pMutateConnectionWeights = .96;
                    np.pMutateDeleteConnection = 0;
                    np.pMutateDeleteSimpleNeuron = 0;
                    np.activationProbabilities = new double[4];
                    np.activationProbabilities[0] = .25;
                    np.activationProbabilities[1] = .25;
                    np.activationProbabilities[2] = .25;
                    np.activationProbabilities[3] = .25;
                    np.populationSize = 150;
                    np.pruningPhaseBeginComplexityThreshold = float.MaxValue;
                    np.pruningPhaseBeginFitnessStagnationThreshold = int.MaxValue;
                    np.pruningPhaseEndComplexityStagnationThreshold = int.MinValue;
                    np.pInitialPopulationInterconnections = 1;
                    np.elitismProportion = .1;
                    np.targetSpeciesCountMax = np.populationSize / 10;
                    np.targetSpeciesCountMin = np.populationSize / 10 - 2;
                    np.selectionProportion = .8;
                    neatParams = np;
                }
                return neatParams;
            }
        }
        public IActivationFunction SuggestedActivationFunction
        {
            get { return HyperNEATParameters.substrateActivationFunction; }
        }
        public string ExplanatoryText
        {
            get { return ExperimentParameters.experimentDescription; }
                //return "A HyperNEAT experiment for Training a Spiking Neural Network model moving a robot arm"; }
        }

        public AbstractExperimentView CreateExperimentView()
        {
            return null;
        }

        public void LoadExperimentParameters(Hashtable parameterTable)
        {
            // is done static with ExperimentParameters
   
        }

        public void ResetEvaluator(IActivationFunction activationFn)
        { 
                populationEvaluator = new StaticPopulationEvaluator(new StaticNetworkEvaluator(neuronTypes));
        }
    }
}
