//#define Windows
//#define Linux
using csmatio.io;
using csmatio.types;
using EsExperimentNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EsExperimentNS.EsSubstrate;
using static EsExperimentNS.Helper;

namespace EsExperimentNS
{
    public class MatlabCommunicator
    {


        public readonly string name;// name has to be unique  
        readonly string syncPath;
        readonly string dataPath;
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
        { //  der thread der blocked muss dann auch "runmodel" aufrufen. 
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
            dataPath = ExperimentParameters.communicationPath + pathSep + @"data_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
            string arg1_name = name;
            string arg2_maxTrainTime = ExperimentParameters.training_maxTrainTime.ToString();
            string arg3_savePath = ExperimentParameters.modelSavePath;
            string arg4_dataPath = dataPath;
            string arg5_resultPath = resultPath;
            string arg6_syncPath = syncPath;
            string arg7_anglesToLearn = ExperimentParameters.training_anglesToLearn;
            string arg8_anglesToSimulate = ExperimentParameters.training_anglesToSimulate;
            string arg9_anglesSimulationTime = ExperimentParameters.training_timePerAngle.ToString();
            string arg10_logPath = logPath;
            

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ExperimentParameters.modelLibPathTrain;
            // todo in Experiment parameters

            // win
#if Windows
            // string arguments = String.Format(@" ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}""  ""{9}"" ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath);
            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')"" -logfile ""{10}"" ", arg0_matCommand, arg1_name, arg2_maxTrainTime, arg3_savePath,arg4_dataPath,arg5_resultPath,arg6_syncPath,arg7_anglesToLearn,arg8_anglesToSimulate,arg9_anglesSimulationTime,arg10_logPath);
            startInfo.FileName = "cmd.exe";
#endif
            //Lin
#if Linux
            string arg11_matlabPath = ExperimentParameters.matlabExecPath;//"/storage/matlab-r2017a/bin/";
            string arguments = String.Format(@" -c '{11}{0} \""runModel(\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\',\'{7}\',\'{8}\',\'{9}\')\"" -logfile '{10}' > /dev/null 2>&1' ", arg0_matCommand, arg1_name, arg2_maxTrainTime, arg3_savePath,arg4_dataPath,arg5_resultPath,arg6_syncPath,arg7_anglesToLearn,arg8_anglesToSimulate,arg9_anglesSimulationTime,arg10_logPath,arg11_matlabPath);
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
            deleteSync(dataPath);
            deleteSync(resultPath);
        }

        public double simulate(int[][] connections,List<NType> types)
        {
            return runModel(connections,  types, false, "");
        }
        public double simulateWithPlot(int[][] connections, List<NType> types, string savename)
        {
            return runModel(connections,types, true, savename);
        }


        public static double startRunModelSimulate(string netName)
        {
            string name = "sim_"+netName;
            string logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + name + ".txt";
            string resultPath = ExperimentParameters.communicationPath + pathSep + @"rmsd_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
            string arg1_name = name;
            string arg2_resultPath = resultPath;
            string arg3_netPath = ExperimentParameters.modelSavePath+ pathSep+netName+".mat";
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
           
            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}')"" -logfile ""{7}"" ", arg0_matCommand, arg1_name, arg2_resultPath,arg3_netPath,arg4_startAngle,arg5_anglesToSimulate,arg6_timePerAngle,arg7_logPath);
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
            double fitness = getSyncFitness(resultPath,null, netName);

            deleteSync(resultPath);


            return fitness;

        }

     

        private double runModel(int[][] connections, List<NType> types, Boolean plot, string saveName)
        {


            writeSyncData(connections, typesToInt(types), dataPath);

            if (plot)
            {
                writeSync("simulate_plot \n" + saveName);
            }
            else
            {
                writeSync("simulate");
            }

            waitForMessage("finished");


            double fitness = getSyncFitness(this.resultPath,matlab,name);
            deleteSync(resultPath);
            deleteSync(dataPath);

            used = false;
            return fitness;
        }

        private static int[] typesToInt(List<NType> types)
        {
           return  types.ConvertAll<int>(value => (int)value).ToArray();
            //NType { D, EM, ES, IM, IS };
        }

        private static void writeSyncData(int[][] connections,int[] types, string dataPath)
        {
            if (connections.Length == 0 || types.Length == 0)
            {
                throw new Exception("connections or types have length 0");
            }
            while (true)
            {
                try
                {
                    List<MLArray> l = new List<MLArray>();
                    MLInt32 weights = new MLInt32("connections", connections);
                    l.Add(weights);
                    MLInt32 intTypes = new MLInt32("types", types,1);
                    l.Add(intTypes);
                    new MatFileWriter(dataPath, l, false);
                    return;
                }
                catch (Exception e)
                {
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


        private static double getSyncFitness(string resultPath, Process matlab, string matlabName) 
        {
            DateTime timeStart = DateTime.Now;
            while (true) 
            {
                try
                {
                    MatFileReader reader1 = new MatFileReader(resultPath);
                    MLDouble r1 = (MLDouble)reader1.GetMLArray("fitness");
                    return r1.GetArray()[0][0];
                }
                catch (Exception e)
                {

                    if((e.GetType() == typeof(MatlabIOException)  || e.GetType() == typeof(FileNotFoundException) )&& matlab == null) { // only when matlab == null cause matlab has exited than allready. It might not write the file  later
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
 
           // Thread.Sleep(15000); // erstmal 15 sekunden schlafen solang geht die berechnung eh mindestens (eher 1.5 min)
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
                    throw new MatlabCrashedException(String.Format("Matlab Instance {0} exited . Create new one",this.name));
                }

                Thread.Sleep(100);
            }
        }

        public void close()
        {
            writeSync("close");
            deleteSync(dataPath);
            deleteSync(resultPath);
        }

        public class MatlabCrashedException : System.Exception
        {
            public MatlabCrashedException() : base() { }
            public MatlabCrashedException(string message) : base(message) { }
        }
    }
}
