using MathWorks.MATLAB.NET.Arrays;
using MatlabModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngoingExperimentNS
{

  public  class OngoingModelWrapper
    {
        OngoingModel model = new OngoingModel();



        public void getModelParams(out double[,] weightsM, out double[,] equationParams, out  int[] esemNum  ,int numNeurons, string weightPath, string eqParamsPath)
        {
            MWCharArray m_weightPath = new MWCharArray(weightPath);
            MWCharArray m_eqParamsPath = new MWCharArray(eqParamsPath);
            MWNumericArray m_numNeurons = new MWNumericArray(numNeurons);
            MWArray[] result = model.getModelParams(3, m_numNeurons, m_weightPath, m_eqParamsPath);
            weightsM = (double[,])result[0].ToArray();
            equationParams = (double[,])result[1].ToArray();
            double[,] esemNum2d = (double[,])result[2].ToArray();
            esemNum = new int[2] { (int)esemNum2d[0, 0], (int)esemNum2d[0, 1] };

        }

        public double runModel(string name, int numNeurons, int runtime, string runSettings, string savepath, double[,] equationParams,double[,] weightsM, double[,] esemWeights, bool isSave)
        {
            MWCharArray m_name = new MWCharArray(name);
            MWCharArray m_savepath = new MWCharArray(savepath);
            MWNumericArray m_runtime = new MWNumericArray(runtime);
            MWNumericArray m_numNeurons = new MWNumericArray(numNeurons);
            MWLogicalArray m_isSave = new MWLogicalArray(isSave);
            MWCharArray m_runSettings = new MWCharArray(runSettings);
            MWNumericArray m_weightM = new MWNumericArray(weightsM);
            MWNumericArray m_equationParams = new MWNumericArray(equationParams);
            MWNumericArray M_esemWeights = new MWNumericArray(esemWeights);
            MWArray rmsd =model.runModel(m_name, m_numNeurons, m_runtime, m_runSettings, m_savepath, m_equationParams, m_weightM, M_esemWeights, m_isSave);
            return ((double[,])rmsd.ToArray())[0, 0]; 
        }
    }
 
}
