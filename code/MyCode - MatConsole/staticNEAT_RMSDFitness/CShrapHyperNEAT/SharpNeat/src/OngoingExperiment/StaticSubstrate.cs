using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;

namespace StaticExperimentNS
{

    public static class StaticSubstrate
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="network"></param>CPPN
        /// <param name="inputs"></param> num substrate Neurons input
        /// <param name="outputs"></param> num substrate Neurons output
        /// <returns></returns>
    public static double[][] getConnections(INetwork network, int inputs, int outputs) //CPPN
        {
            // 2d solution
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange; 

            // if connection == 1  if not connection ==0
            double[][] connections = new double[inputs][];

            float inputDelta = 1f / inputs;
            float outputDelta = 1f / outputs;
           
            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;
            // copied iterations from class Substrate code , TODO get to know the best way to calculate this.

            float[] coordinates = new float[2];
            coordinates[0] = 0f;  //x1
            coordinates[1] = 0f; //x2

            double maxoutput=0;
            double minoutput=1000;

            int count = 0;
            // build substrate with two vertical lines with height 1 and distance 0.5 first line at x=-0.25
            // cause of sigmoidal activation function with f(1) ca 1 und f(-1) ca -1 coordinates should be between 0 and 1 
            for (int i = 0; i < inputs; i++, coordinates[0] += inputDelta)
            {
                connections[i] = new double[outputs];
                coordinates[1] = 0;
                for (int j = 0; j < outputs; j++, coordinates[1] += outputDelta)
                {
                    network.ClearSignals();
                    network.SetInputSignals(coordinates);
                    network.MultipleSteps(iterations);
                    //network.RelaxNetwork(iterations, delta).:: iterates until delta is not exceeded anymore
                    double output = network.GetOutputSignal(0)* weightRange;
                    //if (Math.Abs(output) >= threshold)
                    if (output >= threshold)
                    {                   // scale to weight . TODO check if output is in intervall -1,1
                                        // scaled to positiv value TODO  possible just ignore all negative ones
                                        // weights[i, j] = (Math.Abs(output) - threshold) / (1 - threshold) * weightRange;//Math.Sign(output); // threhold = origin point scaling to weightRange by dividing with highest possible value and mutiplying with weightRange
                        connections[i][j] = 1;
                        count++;
                    }
                    if (output > maxoutput) maxoutput = output;
                    if (output < minoutput)  minoutput = output;
                }
            }
            return connections;

        }

        // 4d solution    
        //           public static double[][] getConnections(INetwork network, int inputs, int outputs) //CPPN  double threshold = HyperNEATParameters.threshold;
        //        { double weightRange = HyperNEATParameters.weightRange;

        //        // if connection == 1  if not connection ==0
        //        double[][] connections = new double[inputs][];

        //        float inputDelta = 1f / inputs;
        //        float outputDelta = 1f / outputs;

        //        int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;
        //        // copied iterations from class Substrate code , TODO get to know the best way to calculate this.

        //        float[] coordinates = new float[4];
        //        coordinates[0] = -0.25f;  //x1
        //            coordinates[1] = -0.5f;  //y1
        //            coordinates[2] = 0.25f; //x2
        //            coordinates[3] = -0.5f;  //y2

        //            int count = 0;
        //            // build substrate with two vertical lines with height 1 and distance 0.5 first line at x=-0.25
        //            // cause of sigmoidal activation function with f(1) ca 1 und f(-1) ca -1 coordinates should be between 0 and 1 
        //            for (int i = 0; i<inputs; i++, coordinates[1] += inputDelta)
        //            {   coordinates[3]=-0.5f;
        //                connections[i] = new double[outputs];
        //                for (int j = 0; j<outputs; j++, coordinates[3] += outputDelta)
        //                {
        //                    network.ClearSignals();
        //                    network.SetInputSignals(coordinates);
        //                    network.MultipleSteps(iterations);
        //                    //network.RelaxNetwork(iterations, delta).:: iterates until delta is not exceeded anymore
        //                    double output = network.GetOutputSignal(0);
        //                    //if (Math.Abs(output) >= threshold)
        //                    if (output >= threshold)
        //                    {                   // scale to weight . TODO check if output is in intervall -1,1
        //                                        // scaled to positiv value TODO  possible just ignore all negative ones
        //                                        // weights[i, j] = (Math.Abs(output) - threshold) / (1 - threshold) * weightRange;//Math.Sign(output); // threhold = origin point scaling to weightRange by dividing with highest possible value and mutiplying with weightRange
        //                        connections[i][j] = 1;
        //                        count++;
        //                    }
        //}
        //            }
        //            return connections;

        //   }


    }


}
