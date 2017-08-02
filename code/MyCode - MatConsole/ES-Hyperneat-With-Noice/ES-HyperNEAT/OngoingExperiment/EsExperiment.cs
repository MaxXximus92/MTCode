using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;


namespace EsExperimentNS
{
    public class EsExperiment : IExperiment
    {

        IPopulationEvaluator populationEvaluator = null;
        NeatParameters neatParams = null;
    

        public EsExperiment()
        {


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
                return (int)ExperimentParameters.cPPNInputs;
            }
        }

        public int OutputNeuronCount
        {
            get
            {
                return (int)ExperimentParameters.cPPNOutputs;
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
                    np.pMutateAddConnection = .15; // original 0,03
                    np.pMutateAddNode = .05; // original 0.01
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
                populationEvaluator = new EsPopulationEvaluator(new EsNetworkEvaluator());
        }
    }
}
