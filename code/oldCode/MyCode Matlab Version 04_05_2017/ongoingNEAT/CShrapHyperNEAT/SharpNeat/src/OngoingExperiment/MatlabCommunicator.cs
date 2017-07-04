using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OngoingExperimentNS
{
   public class MatlabCommunicator
    {
        public readonly MLApp.MLApp matlab;
        readonly string matlabDirectoryPath;
        bool modelGenerated = false;
        bool living;
    public MatlabCommunicator(string matlabDirectoryPath)
        {
            Type MatlabType = Type.GetTypeFromProgID("Matlab.Application.single");
            matlab = (MLApp.MLApp)Activator.CreateInstance(MatlabType);
   
           // matlab = new MLApp.MLApp();
            this.matlabDirectoryPath = matlabDirectoryPath;
            matlab.Execute(@"cd '"+matlabDirectoryPath+@"';");
         //   matlab.MaximizeCommandWindow();
            living = true;
        }
        ~MatlabCommunicator()
        {
            if (living) matlab.Quit();
        }
        public void generateModel(uint numNeurons =256, uint spikingThreshold = 30, string matlabSavePath ="")
        {
            if (!living) throw new InvalidOperationException("matlab instance already closed");
            if (modelGenerated && living) return;

            Type a= matlab.GetType();
            // 
            matlab.Execute(String.Format("net = spikenet({0},{1},'settings.xls','{2}');",numNeurons, spikingThreshold,matlabSavePath));
            modelGenerated = true;

        }
    public double[,] getEsEmWeightsMatrix()
        {
            checkIfCallValid();

            matlab.Execute("esEmWeights = net.getEsEmWeights();");
                matlab.Execute("dims = size(esEmWeights);");
                double[,] dims = matlab.GetVariable("dims", "base"); // returns double[1,2]
                System.Array weights = new double[(int)dims[0, 0], (int)dims[0, 1]];
                System.Array dummy = new double[0, 0];
                matlab.GetFullMatrix("esEmWeights", "base", ref weights, ref dummy);
                return (double[,])weights;

        }
        public void setEsEmWeightsMatrix(double [,] weightMatrix)
        {
            checkIfCallValid();
            //matlab.PutFullMatrix("EsEmWeightsToSet","base",weightMatrix, new double[0, 0]);
            matlab.PutWorkspaceData("EsEmWeightsToSet", "base", weightMatrix);
           matlab.Execute("net.setEsEmWeights(EsEmWeightsToSet);");
 
        }

        public double startSimulation(uint runtime = 1200, string runSettings = @"[1;35]",uint trailsPerSetting =1 ,bool plotGraphs = false )
        {
            checkIfCallValid();

            matlab.Execute(String.Format("rmsds = net.simulate(1,{0}, {1}, {3},{2},{2});", runtime, runSettings.ToString().ToLower(), plotGraphs.ToString().ToLower(),trailsPerSetting));
            double rmsd = matlab.GetVariable("rmsds", "base");
            return rmsd;
        }

        public void saveModel(string name)
        {
            matlab.Execute(String.Format("net.save({0});",name));
        }

        public void resetModel()
        {
            matlab.Execute(String.Format("net.reset();"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>index 0 = Number es cells; Index 1 = Number em </returns>
        public int[] getEsEmNum()
        {
            checkIfCallValid();
            matlab.Execute(String.Format("esEmNum = net.getEsEmNum()"));
            double[,] ret = matlab.GetVariable("esEmNum", "base");
            int[] esEmNum = { (int)ret[0, 0], (int)ret[0, 1] };
            return esEmNum;
        }

        public void close()
        {
            matlab.Quit();
            living = false;
        }

        private void checkIfCallValid()
        {
            if(living !=true)
                throw new InvalidOperationException("matlab instance allready closed");
            if (modelGenerated != true)
                throw new InvalidOperationException("no model generated yet");
        }
    }

}
