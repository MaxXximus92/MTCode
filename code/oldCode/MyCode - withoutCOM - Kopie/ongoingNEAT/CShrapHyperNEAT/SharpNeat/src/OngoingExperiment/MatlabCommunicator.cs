//#define Windows
//#define Linux
using csmatio.io;
using csmatio.types;
using OngoingExperimentNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OngoingExperimentNS.Helper;

namespace OngoingExperimentNS
{
    public class MatlabCommunicator
    {


        public readonly string name;// name has to be unique  oder counter...
        readonly string syncPath;
        readonly string weightPath;
        readonly string resultPath;
        bool used = false;

      
        Semaphore sem = new Semaphore(1, 1);
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if block was possible</returns>
        public bool block()
        { //  der thread der blocked muss dann auch "startrunmodel" aufrufen. 
            sem.WaitOne();
            if (used == true)
            {
                sem.Release();
                return false;
            }
            used = true;
            sem.Release();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"> has to be unique 1</param>
        /// <param name="processNumber"></param> has to start by 0
        /// 
        public MatlabCommunicator(string name)
        {
            this.name = name;
            syncPath =  ExperimentParameters.communicationPath + pathSep + @"sync_" + name + ".txt";
            resultPath =  ExperimentParameters.communicationPath + pathSep + @"rmsd_" + name + ".mat";
            weightPath =  ExperimentParameters.communicationPath + pathSep + @"EsEmWeights_" + name + ".mat";



            string arg1_name = name;
            string arg2_numNeurons = ExperimentParameters.numNeurons.ToString();
            string arg3_runTime = ExperimentParameters.runTime.ToString();
            string arg4_runSettings = ExperimentParameters.runSettings;
            string arg5_savePath = ExperimentParameters.matlabSavePath;
            string arg6_equParamPath =  ExperimentParameters.initialNetEqParamsPath;
            string arg7_weightsMPath =  ExperimentParameters.initialNetWeightsPath;
            string arg8_esemConPath =  weightPath;
            string arg9_resultPath =  resultPath;
            string arg10_syncPath =  syncPath;


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            // todo in Experiment parameters

            // win
#if Windows
            string arguments = String.Format(@" ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}""  ""{9}"" ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath);
#endif
            //Lin
#if Linux
            string arg11_matlabPath = "/storage/matlab-r2016b";
            string arguments = String.Format(@" {10} ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}"" ""{9}""  ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath, arg11_matlabPath);
#endif

            startInfo.FileName = ExperimentParameters.matlabExecPathRunModel;
            startInfo.Arguments = arguments;

            //generate sync file
            writeSync("initialized");

            Thread.BeginThreadAffinity();
            Process exeProcess = Process.Start(startInfo);


            Thread.EndThreadAffinity();


        }
        ~MatlabCommunicator()
        {
            writeSync("close");
           // deleteSync(syncPath); -> gets deleted by matlab
            deleteSync(weightPath);
            deleteSync(resultPath);
        }

        public double simulate(double[][] esemWeights)
        {
            return startRunModel(esemWeights, false, "");
        }
        public double simulateWithPlot(double[][] esemWeights, string savename)
        {
            return startRunModel(esemWeights, true, savename);
        }

        /// <summary>
        /// Starts a new Matalb getParams instance 
        /// </summary>
        /// <param name="CommunicationName"></param>
        /// <returns>esemNum es=[0]  em=[1]</returns>
        public static int[] getEsEMNum(string CommunicationName)
        {
            string resultPath =  ExperimentParameters.communicationPath + pathSep + @"esemNum_" + CommunicationName + ".mat";

            string arg1_numNeurons = ExperimentParameters.numNeurons.ToString();
            string arg2_resultPath = resultPath;
            // win
#if Windows
            string arguments = String.Format(@" ""{0}""  ""{1}"" ", arg1_numNeurons,  arg2_resultPath);
            //lin
#endif
#if Linux
            string arg3_matlabPath = "/storage/matlab-r2016b";
           string arguments = String.Format(@" {2} ""{0}"" ""{1}""  ", arg1_numNeurons, arg2_resultPath, arg3_matlabPath);
#endif

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            startInfo.FileName = ExperimentParameters.matlabExecPathGetParams;

            startInfo.Arguments = arguments;

           // Console.WriteLine("Starting script:");
           // Console.WriteLine(startInfo.FileName + " "+ arguments);


            // Start the process with the info we specified.
            // Call WaitForExit and then the using-statement will close.
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }
            MatFileReader reader = new MatFileReader(resultPath);
            MLDouble result = (MLDouble)reader.GetMLArray("esemNum");
            double[] esemNum = result.GetArray()[0];
            deleteSync(resultPath);
            return new int[] { (int)esemNum[0], (int)esemNum[1] };
        }

        private double startRunModel(double[][] esemWeights, Boolean plot, string saveName)
        {


            writeSyncMatrix(esemWeights);
            if (plot)
            {
                writeSync("simulate_plot \n" + saveName); 
            }
            else
            {
                writeSync("simulate");
            }

            waitForMessage("finished");


            double rmsd = getSyncRmsd();
            deleteSync(resultPath);
            deleteSync(weightPath);

            used = false;


            return rmsd;
        }



        private void writeSyncMatrix(double[][]  esemWeights)
        {
            while (true)
            {
                try
                {
                    MLDouble weights = new MLDouble("esemConnections", esemWeights);
                    List<MLArray> l = new List<MLArray>();
                    l.Add(weights);
                    new MatFileWriter(weightPath, l, false);
                    return;
                }
                catch (Exception e) 
                {
                    if (e.GetType() == typeof(FileNotFoundException))
                    {
                        throw e;
                    }
                    // Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }

                Thread.Sleep(100);
            }

        }


        private double getSyncRmsd()
        {

            while (true)
            {
                try
                {
                    MatFileReader reader1 = new MatFileReader(resultPath);
                    MLDouble r1 = (MLDouble)reader1.GetMLArray("rmsd");
                    return  r1.GetArray()[0][0];
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(FileNotFoundException))
                    {
                        throw e;
                    }
                    // Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }

                Thread.Sleep(100);
            }

        }

    


        private static void deleteSync(string path)
        {
            bool done = false;


            while (!done)
            {

                try
                {
                    File.Delete(path);
                    done = true;
                }
                catch (Exception e) // todo check for file not found exception
                {
                    if (e.GetType() == typeof(FileNotFoundException))
                    {
                        throw e;
                    }
                    done = false;
                    // Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }

                Thread.Sleep(100);
            }

        }
        private void writeSync(string message)
        {
            bool done = false;
         

            while (!done)
            {
                StreamWriter sync_writer= null;
                try
                {
                    sync_writer = new System.IO.StreamWriter(syncPath, false);
                    sync_writer.Write(message);
                    sync_writer.Flush();
                    sync_writer.Close();
                    done = true;
                }
                catch (Exception e) // todo check for file not found exception
                {
                    if(e.GetType() == typeof(FileNotFoundException)){
                        throw e;
                    }
                    done = false;
                   // Console.WriteLine(e.Message);
                   // Console.WriteLine("trying again");
                }
                finally
                {
                    if (sync_writer != null)
                    {
                        sync_writer.Close();
                    }
                }
                Thread.Sleep(100);
            }

        }

        private void waitForMessage(string message)
        {
            Thread.Sleep(15000); // erstmal 15 sekunden schlafen solang geht die berechnung eh mindestens (eher 1.5 min)
            while (true)
            {

                StreamReader sync_reader = null; ;
                try
                {
                     sync_reader = new StreamReader(syncPath);
                    if (sync_reader.ReadLine() == message) {
                        sync_reader.Close();
                        break; }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(FileNotFoundException))
                    {
                        throw e;
                    }
                    //Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }
                finally
                {
                    if(sync_reader != null)
                    {
                        sync_reader.Close();
                    }
                }
                
                Thread.Sleep(100);
            }
        }
    }
}
