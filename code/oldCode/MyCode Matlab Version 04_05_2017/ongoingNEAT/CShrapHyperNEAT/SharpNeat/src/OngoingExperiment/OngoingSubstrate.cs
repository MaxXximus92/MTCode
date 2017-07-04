using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;

namespace OngoingExperimentNS
{

    static class OngoingSubstrate
    {



    public static double[,] getWeights(INetwork network, int inputs, int outputs) //CPPN
        {
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange;

            double[,] weights = new double[inputs, outputs];

            float inputDelta = 100 / inputs;
            float outputDelta = 100 / outputs;
           
            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;
            // copied iterations from class Substrate code , TODO get to know the best way to calculate this.

            float[] coordinates = new float[4];
            coordinates[0] = -25;  //x1
            coordinates[1] = 0;  //y1
            coordinates[2] = 25; //x2
            coordinates[3] = 0;  //y2
            // build substrate with two vertical lines with height 100 and distance 50 firt line at x=-25
            for (int i = 0; i < inputs; i++, coordinates[1] += inputDelta)
            {
                for (int j = 0; j < outputs; j++, coordinates[3] += outputDelta)
                {
                    network.ClearSignals();
                    network.SetInputSignals(coordinates);
                    network.MultipleSteps(iterations);
                    //network.RelaxNetwork(iterations, delta).:: iterates until delta is not exceeded anymore
                    float output = network.GetOutputSignal(0);
                    if (Math.Abs(output) >= threshold)
                    {                   // scale to weight . TODO check if output is in intervall -1,1
                                                                                                         // scaled to positiv value TODO  possible just ignore all negative ones
                        weights[i, j] = (Math.Abs(output) - threshold) / (1 - threshold) * weightRange;//Math.Sign(output); // threhold = origin point scaling to weightRange by dividing with highest possible value and mutiplying with weightRange
                    }
                }
            }
            return weights;

        }
    }


}
