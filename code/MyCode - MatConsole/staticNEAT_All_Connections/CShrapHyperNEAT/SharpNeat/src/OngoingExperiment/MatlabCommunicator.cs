﻿//#define Windows
//#define Linux
using csmatio.io;
using csmatio.types;
using StaticExperimentNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StaticExperimentNS.Helper;

namespace StaticExperimentNS
{
    public class MatlabCommunicator
    {


        public readonly string name;// name has to be 
        readonly string syncPath;
        readonly string weightPath;
        readonly string logPath;
        readonly string resultPath;
        readonly Process matlab;
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
            logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + name + ".txt";
            syncPath = ExperimentParameters.communicationPath + pathSep + @"sync_" + name + ".txt";
            resultPath = ExperimentParameters.communicationPath + pathSep + @"rmsd_" + name + ".mat";
            weightPath = ExperimentParameters.communicationPath + pathSep + @"connections_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
            string arg1_name = name;
            string arg2_maxTrainTime = ExperimentParameters.training_maxTrainTime.ToString();
            string arg3_savePath = ExperimentParameters.modelSavePath;
            string arg4_equParamPath = ExperimentParameters.initialNetEqParamsPath;
            string arg5_esemConPath = weightPath;
            string arg6_resultPath = resultPath;
            string arg7_syncPath = syncPath;
            string arg8_anglesToLearn = ExperimentParameters.training_anglesToLearn;
            string arg9_anglesToSimulate = ExperimentParameters.training_anglesToSimulate;
            string arg10_anglesSimulationTime = ExperimentParameters.training_timePerAngle.ToString();
            string arg11_logPath = logPath;


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ExperimentParameters.modelLibPathTrain;
            // todo in Experiment parameters

            // win
#if Windows
            // string arguments = String.Format(@" ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}""  ""{9}"" ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath);
            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')"" -logfile ""{11}"" ", arg0_matCommand, arg1_name, arg2_maxTrainTime, arg3_savePath, arg4_equParamPath, arg5_esemConPath, arg6_resultPath, arg7_syncPath, arg8_anglesToLearn, arg9_anglesToSimulate, arg10_anglesSimulationTime, arg11_logPath);
            startInfo.FileName = "cmd.exe";
#endif
            //Lin
#if Linux
            string arg12_matlabPath = ExperimentParameters.matlabExecPath;//"/storage/matlab-r2017a/bin/";
            string arguments = String.Format(@" -c '{12}{0} \""runModel(\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\',\'{7}\',\'{8}\',\'{9}\',\'{10}\')\"" -logfile '{11}' > /dev/null 2>&1' ", arg0_matCommand, arg1_name, arg2_maxTrainTime, arg3_savePath, arg4_equParamPath, arg5_esemConPath, arg6_resultPath, arg7_syncPath, arg8_anglesToLearn, arg9_anglesToSimulate, arg10_anglesSimulationTime, arg11_logPath, arg12_matlabPath);
            startInfo.FileName = "/bin/bash";
#endif

            // startInfo.FileName = ExperimentParameters.matlabExecPathRunModel;
            startInfo.Arguments = arguments;

            //generate sync file
            writeSync("initialized");

            Thread.BeginThreadAffinity();
            Process exeProcess = Process.Start(startInfo);
            matlab = exeProcess;

            Thread.EndThreadAffinity(); // TODO test if better without


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


        public static double startRunModelSimulate(string netName)
        {
            string name = "sim_" + netName;
            string logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + name + ".txt";
            string resultPath = ExperimentParameters.communicationPath + pathSep + @"rmsd_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
            string arg1_name = name;
            string arg2_resultPath = resultPath;
            string arg3_netPath = ExperimentParameters.modelSavePath + pathSep + netName + ".mat";
            string arg4_startAngle = ExperimentParameters.startAngle.ToString();
            string arg5_anglesToSimulate = ExperimentParameters.simulation_anglesToSimulate;
            string arg6_timePerAngle = ExperimentParameters.simulation_timePerAngle.ToString();
            string arg7_logPath = logPath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = ExperimentParameters.modelLibPathSimulate;


            // win
#if Windows

            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}')"" -logfile ""{7}"" ", arg0_matCommand, arg1_name, arg2_resultPath, arg3_netPath, arg4_startAngle, arg5_anglesToSimulate, arg6_timePerAngle, arg7_logPath);
            startInfo.FileName = "cmd.exe";
#endif

#if Linux
            string arg8_matlabPath = ExperimentParameters.matlabExecPath;//"/storage/matlab-r2017a/bin/";
            string arguments = String.Format(@" -c '{8}{0} \""runModel(\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\')\"" -logfile '{7}' > /dev/null 2>&1 ' ", arg0_matCommand, arg1_name, arg2_resultPath,arg3_netPath,arg4_startAngle,arg5_anglesToSimulate,arg6_timePerAngle,arg7_logPath,arg8_matlabPath);
            startInfo.FileName = "/bin/bash";
#endif

            // startInfo.FileName = ExperimentParameters.matlabExecPathRunModel;
            startInfo.Arguments = arguments;

            Thread.BeginThreadAffinity();
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }
            Thread.EndThreadAffinity();
            //Console.WriteLine("Simulation process exited");
            double fitness = getSyncFitness(resultPath, null, netName);

            deleteSync(resultPath);


            return fitness;

        }

        /// <summary>
        /// Starts a new Matalb getParams instance 
        /// </summary>
        /// <param name="communicationName"></param>
        /// <returns>esemNum es=[0]  em=[1]</returns>
        public static ModelNeuronType[] getNeuronTypes(string communicationName)
        {
            string logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + communicationName + ".txt";
            string resultPath = ExperimentParameters.communicationPath + pathSep + @"dEsNum_" + communicationName + ".mat";


            string arg1_resultPath = resultPath;
            string arg2_logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + "getParams" + ".txt";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ExperimentParameters.modelLibPathTrain;

            // win
#if Windows
            string arg0_matcommand = "matlab -wait -nodesktop -nodisplay -nosplash -nojvm -singleCompThread  -r ";
            string arguments = String.Format(@" /C {0} ""getModelParams('{1}')""  -logfile {2} ", arg0_matcommand, arg1_resultPath, arg2_logPath);
            startInfo.FileName = "cmd.exe";
#endif
#if Linux
            string arg0_matcommand = "matlab -nodesktop -nodisplay -nosplash -nojvm -singleCompThread  -r ";
            string arg3_matlabPath = ExperimentParameters.matlabExecPath;

            string arguments = String.Format(@" -c '{3}{0} \""getModelParams(\'{1}\')\""  -logfile '{2}' > /dev/null 2>&1' ", arg0_matcommand, arg1_resultPath, arg2_logPath, arg3_matlabPath);
            startInfo.FileName = "/bin/bash";
#endif


            // todo in Experiment parameters


            startInfo.Arguments = arguments;

           // Console.WriteLine("Starting script:");
           // Console.WriteLine(startInfo.FileName + " "+ arguments);


            // Start the process with the info we specified.
            // Call WaitForExit and then the using-statement will close.
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit(); 
            }

            //%'D' =0, 'EM'=1, 'ES'=2, 'IM'=3 'IS'=4
            MatFileReader reader = new MatFileReader(resultPath);
            MLDouble result =(MLDouble)reader.GetMLArray("neuronTypes");
            double[] types = result.GetArray()[0];
            ModelNeuronType[] enumTypes = new ModelNeuronType[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                enumTypes[i] = (ModelNeuronType)Convert.ToInt32(types[i]);
            }
            Console.WriteLine("loaded types' length:"+ types.Length);
            deleteSync(resultPath);
            return enumTypes;
        }

        public enum ModelNeuronType { D,EM,ES,IM,IS};

        private double startRunModel(double[][] connections, Boolean plot, string saveName)
        {


            writeSyncMatrix(connections, weightPath);
            if (plot)
            {
                writeSync("simulate_plot \n" + saveName); 
            }
            else
            {
                writeSync("simulate");
            }

            waitForMessage("finished");


            double fitness = getSyncFitness(this.resultPath, matlab, name);
            deleteSync(resultPath);
            deleteSync(weightPath);

            used = false;


            return fitness;
        }



        private static void writeSyncMatrix(double[][]  connections, string weightPath)
        {
            while (true)
            {
                try
                {
                    MLDouble weights = new MLDouble("connections", connections);
                    List<MLArray> l = new List<MLArray>();
                    l.Add(weights);
                    new MatFileWriter(weightPath, l, false);
                    return;
                }
                catch (Exception e) 
                {
                    Type a = e.GetType();
                    if (e.GetType() == typeof(FileNotFoundException)|| e.GetType() == typeof(System.IO.PathTooLongException))
                    {
                        throw e;
                    }
                    // Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }

                Thread.Sleep(100);
            }

        }


        private static double getSyncFitness(string resultPath, Process matlab, string matlabName)
        {
            DateTime timeStart = DateTime.Now;
            while (true) // TOOD refactor method
            {
                try
                {
                    MatFileReader reader1 = new MatFileReader(resultPath);
                    MLDouble r1 = (MLDouble)reader1.GetMLArray("fitness");
                    return r1.GetArray()[0][0];
                }
                catch (Exception e)
                {

                    if (e.GetType() == typeof(MatlabIOException) && matlab == null)
                    { // only when matlab == null cause matlab has exited than allready. It might not write the file  later
                        throw new MatlabCrashedException(String.Format("fitness file not readable, Matlab {0} must have been crashed before writing it", matlabName));
                    }
                    //Console.WriteLine(e.GetType().ToString());
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine("trying again");
                }

                if (matlab != null && matlab.HasExited)
                {
                    throw new MatlabCrashedException(String.Format("Matlab Instance {0} exited . Create new one", matlabName));
                }

                Thread.Sleep(100);
            }

        }




        private static void deleteSync(string path)
        {


            while (true)
            {

                try
                {
                    File.Delete(path); // throws no error if file does not exist
                    return;
                }
                catch (IOException e)
                {

                    // Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }

                Thread.Sleep(100);
            }

        }
        private void writeSync(string message)
        {

            while (true)
            {
                StreamWriter sync_writer = null;
                try
                {
                    sync_writer = new System.IO.StreamWriter(syncPath, false);
                    sync_writer.Write(message);
                    sync_writer.Flush();
                    sync_writer.Close();
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

                //Console.WriteLine("matlab closed ? {0}", matlab.HasExited);
                StreamReader sync_reader = null; ;
                try
                {
                    sync_reader = new StreamReader(syncPath);
                    if (sync_reader.ReadLine() == message)
                    {
                        sync_reader.Close();
                        return;
                    }
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
                    if (sync_reader != null)
                    {
                        sync_reader.Close();
                    }
                }

                if (matlab.HasExited)
                {
                    throw new MatlabCrashedException(String.Format("Matlab Instance {0} exited . Create new one", this.name));
                }

                Thread.Sleep(100);
            }
        }

        public void close()
        {
            writeSync("close");
            deleteSync(weightPath);
            deleteSync(resultPath);
        }


        public class MatlabCrashedException : System.Exception
        {
            public MatlabCrashedException() : base() { }
            public MatlabCrashedException(string message) : base(message) { }
        }
        static public void writeWeightMatrices(List<double[][]> weights, string safePath  )
        {
            while (true)
            {
                try
                {
                    MLDouble DEs = new MLDouble("DEsWeights", weights[0]);
                    MLDouble EsIs = new MLDouble("EsIsWeights", weights[1]);
                    MLDouble IsIs = new MLDouble("IsIsWeights", weights[2]);
                    MLDouble IsEs = new MLDouble("IsEsWeights", weights[3]);
                    MLDouble EsEm = new MLDouble("EsEmWeights", weights[4]);
                    MLDouble EmIm = new MLDouble("EmImWeights", weights[5]);
                    MLDouble ImIm = new MLDouble("ImImWeights", weights[6]);
                    MLDouble ImEm = new MLDouble("ImEmWeights", weights[7]);

                    List<MLArray> l = new MLArray[] {DEs,EsIs,IsIs,IsEs,EsEm,EmIm,ImIm,ImEm}.ToList();
                    new MatFileWriter(safePath, l, false);
                    return;
                }
                catch (Exception e)
                {
                    Type a = e.GetType();
                    if (e.GetType() == typeof(FileNotFoundException) || e.GetType() == typeof(System.IO.PathTooLongException))
                    {
                        throw e;
                    }
                    // Console.WriteLine(e.Message);
                    // Console.WriteLine("trying again");
                }

                Thread.Sleep(100);
            }
        }
    }
}
