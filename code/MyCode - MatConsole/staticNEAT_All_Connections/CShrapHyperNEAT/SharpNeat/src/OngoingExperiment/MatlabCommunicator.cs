//#define Windows
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


        public readonly string name;// name has to be unique  oder counter...
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
            syncPath =  ExperimentParameters.communicationPath + pathSep + @"sync_" + name + ".txt";
            resultPath =  ExperimentParameters.communicationPath + pathSep + @"rmsd_" + name + ".mat";
            weightPath =  ExperimentParameters.communicationPath + pathSep + @"dEsWeights_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
            string arg1_name = name;
            string arg2_maxRunTime = ExperimentParameters.maxRuntime.ToString();
            string arg3_savePath = ExperimentParameters.modelSavePath;
            string arg4_equParamPath =  ExperimentParameters.initialNetEqParamsPath;
            string arg5_esemConPath =  weightPath;
            string arg6_resultPath =  resultPath;
            string arg7_syncPath =  syncPath;
            string arg8_logPath = logPath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
           // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ExperimentParameters.modelLibPathTrain;
            // todo in Experiment parameters

            // win
#if Windows
            // string arguments = String.Format(@" ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}""  ""{9}"" ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath);
            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}','{7}')"" -logfile ""{8}"" ", arg0_matCommand, arg1_name, arg2_maxRunTime, arg3_savePath, arg4_equParamPath, arg5_esemConPath, arg6_resultPath, arg7_syncPath, arg8_logPath);
            startInfo.FileName = "cmd.exe";
#endif
            //Lin
#if Linux
            string arg9_matlabPath = ExperimentParameters.matlabExecPath;//"/storage/matlab-r2017a/bin/";
            string arguments = String.Format(@" -c '{9}{0} \""runModel(\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\',\'{7}\')\"" -logfile '{8}' > /dev/null 2>&1 ' ", arg0_matCommand, arg1_name, arg2_maxRunTime, arg3_savePath, arg4_equParamPath, arg5_esemConPath, arg6_resultPath, arg7_syncPath, arg8_logPath, arg9_matlabPath);
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


        public static double startRunModelSimulate(double[][] DEsWeights,string communicationName)
        {
            string name = communicationName;
            string logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + name + ".txt";
            string resultPath = ExperimentParameters.communicationPath + pathSep + @"rmsd_" + name + ".mat";
            string weightPath = ExperimentParameters.communicationPath + pathSep + @"dEsWeights_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
            string arg1_name = name;
            string arg2_savePath = ExperimentParameters.modelSavePath;
            string arg3_equParamPath = ExperimentParameters.initialNetEqParamsPath;
            string arg4_conPath = weightPath;
            string arg5_resultPath = resultPath;
            string arg6_startAngle = ExperimentParameters.startAngle.ToString();
            string arg7_anglesToSimulate = ExperimentParameters.anglesToSimulate;
            string arg8_timePerAngle = ExperimentParameters.timePerAngle.ToString();
            string arg9_logPath = logPath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = ExperimentParameters.modelLibPathSimulate;


            // win
#if Windows
            // string arguments = String.Format(@" ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}""  ""{9}"" ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath);
            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')"" -logfile ""{9}"" ", arg0_matCommand, arg1_name, arg2_savePath, arg3_equParamPath, arg4_conPath, arg5_resultPath,arg6_startAngle,arg7_anglesToSimulate,arg8_timePerAngle ,arg9_logPath);
            startInfo.FileName = "cmd.exe";
#endif

#if Linux
            string arg10_matlabPath = ExperimentParameters.matlabExecPath;//"/storage/matlab-r2017a/bin/";
            string arguments = String.Format(@" -c '{10}{0} \""runModel(\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\',\'{7}\',\'{8}\')\"" -logfile '{9}' > /dev/null 2>&1 ' ", arg0_matCommand, arg1_name, arg2_savePath, arg3_equParamPath, arg4_weightsMPath, arg4_conPath, arg5_resultPath, arg6_startAngle, arg7_anglesToSimulate, arg8_timePerAngle, arg9_logPath, arg10_matlabPath);
            startInfo.FileName = "/bin/bash";
#endif

            // startInfo.FileName = ExperimentParameters.matlabExecPathRunModel;
            startInfo.Arguments = arguments;

            writeSyncMatrix(DEsWeights, weightPath);
            Thread.BeginThreadAffinity();
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }
            Thread.EndThreadAffinity();

            double fitness = getSyncFitness(resultPath, null, communicationName);

            deleteSync(resultPath);
            deleteSync(weightPath);

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
            string resultPath =  ExperimentParameters.communicationPath + pathSep + @"neuronTypes_" + communicationName + ".mat";


            string arg1_resultPath = resultPath;
            string arg2_logPath = resultPath;

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
            arg2_logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + "getParams" + ".txt";  
            //string arguments = String.Format(@" -c $'{3}{0} \""getModelParams(\'{1}\',\'{2}\')\""  -logfile 'log_getParams' ' ", arg0_matcommand,arg1_numNeurons, arg2_resultPath, arg3_matlabPath);
            string arguments = String.Format(@" -c '{3}{0} \""getModelParams(\'{1}\')\""  -logfile '{2}' > /dev/null 2>&1 ' ", arg0_matcommand, arg1_resultPath,arg2_logPath, arg3_matlabPath);
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
            Console.WriteLine("Loaded Types length:"+ types.Length);
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
    }
}
