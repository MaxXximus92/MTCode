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
        readonly string logPath;
        readonly string resultPath;
        readonly Process matlab_process;
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
            weightPath =  ExperimentParameters.communicationPath + pathSep + @"EsEmWeights_" + name + ".mat";

            string arg0_matCommand = "matlab  -nodesktop -nodisplay -nosplash -singleCompThread -r ";
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
            string arg11_logPath = logPath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
           // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ExperimentParameters.matlabPath;
            // todo in Experiment parameters

            // win
#if Windows
            // string arguments = String.Format(@" ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" ""{6}"" ""{7}"" ""{8}""  ""{9}"" ", arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath);
            string arguments = String.Format(@" /C {0} ""runModel('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')"" -logfile ""{11}"" ", arg0_matCommand, arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath, arg11_logPath);
            startInfo.FileName = "cmd.exe";
#endif
            //Lin
#if Linux
            string arg12_matlabPath = "
            ";
            string arguments = String.Format(@" -c '{12}{0} \""runModel(\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\',\'{7}\',\'{8}\',\'{9}\',\'{10}\')\"" -logfile '{11}' > /dev/null 2>&1 ' ", arg0_matCommand, arg1_name, arg2_numNeurons, arg3_runTime, arg4_runSettings, arg5_savePath, arg6_equParamPath, arg7_weightsMPath, arg8_esemConPath, arg9_resultPath, arg10_syncPath, arg11_logPath, arg12_matlabPath);
            startInfo.FileName = "/bin/bash";
#endif

            // startInfo.FileName = ExperimentParameters.matlabExecPathRunModel;
            startInfo.Arguments = arguments;

            //generate sync file
            writeSync("initialized");

            Thread.BeginThreadAffinity();
            matlab_process = Process.Start(startInfo);


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
            string logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + CommunicationName + ".txt";
            string resultPath =  ExperimentParameters.communicationPath + pathSep + @"esemNum_" + CommunicationName + ".mat";
           


            string arg1_numNeurons = ExperimentParameters.numNeurons.ToString();
            string arg2_resultPath = resultPath;
            string arg3_logPath = resultPath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
           // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = ExperimentParameters.matlabPath;

            // win
#if Windows
             string matcommand = "matlab -wait -nodesktop -nodisplay -nosplash -nojvm -singleCompThread  -r ";
            string arguments = String.Format(@" /C {0} ""getModelParams('{1}','{2}')""  -logfile {3} ", matcommand, arg1_numNeurons, arg2_resultPath, arg3_logPath);
            startInfo.FileName = "cmd.exe";
#endif
#if Linux
            string arg0_matcommand = "matlab -nodesktop -nodisplay -nosplash -nojvm -singleCompThread  -r ";
            string arg3_matlabPath = "/storage/matlab-r2017a/bin/";
            logPath = ExperimentParameters.communicationPath + pathSep + @"log_" + "getParams" + ".txt";  
            //string arguments = String.Format(@" -c $'{3}{0} \""getModelParams(\'{1}\',\'{2}\')\""  -logfile 'log_getParams' ' ", arg0_matcommand,arg1_numNeurons, arg2_resultPath, arg3_matlabPath);
            string arguments = String.Format(@" -c '{3}{0} \""getModelParams(\'{1}\',\'{2}\')\""  -logfile '{4}' > /dev/null 2>&1 ' ", arg0_matcommand, arg1_numNeurons, arg2_resultPath, arg3_matlabPath,logPath);
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

   
            MatFileReader reader = new MatFileReader(resultPath);
            MLDouble result = (MLDouble)reader.GetMLArray("esemNum");
            double[] esemNum = result.GetArray()[0];
            Console.WriteLine("Loaded EsEmNum: {0} {1}", (int)esemNum[0], (int)esemNum[1]);
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
            DateTime timeStart = DateTime.Now;
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
                double tDelta = (DateTime.Now.Subtract(timeStart)).TotalMilliseconds;
                if (tDelta >= ExperimentParameters.maxModelCompTime)
                {
                    throw new MatlabNotRespondingException(String.Format("Model needed longer than maximal computation time to evaluate. Matlabinstance must be broken"));
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
            DateTime timeStart = DateTime.Now;
           // Console.WriteLine("TimeStart:" +timeStart.ToString());
            int counter = 0;
         //   Thread.Sleep(15000); // erstmal 15 sekunden schlafen solang geht die berechnung eh mindestens (eher 1.5 min)
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
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                   // Console.WriteLine(e.Message);
                   // Console.WriteLine("trying again");
                }
                finally
                {
                    if(sync_reader != null)
                    {
                        sync_reader.Close();
                    }
                }
                counter++;
               
                double tDelta = (DateTime.Now.Subtract(timeStart)).TotalMilliseconds;
               // if (counter % 100 == 0) { Console.WriteLine("tDelta=" + tDelta);
               //     Console.WriteLine("TimeStart:"+ DateTime.Now.ToLongTimeString());
               // }
                if (tDelta >= ExperimentParameters.maxModelCompTime)
                {
                    throw new MatlabNotRespondingException(String.Format("Model needed longer than maximal computation time to evaluate. Matlabinstance must be broken"));
                }

                Thread.Sleep(100);
            }
        }
        
        public class MatlabNotRespondingException : System.Exception
        {
            public MatlabNotRespondingException() : base() { }
            public MatlabNotRespondingException(string message) : base(message) { }
        }


        public void close()
        {
            writeSync("close");
        }



//        public void kill() // wenn bereits tot stürzt das ganze programm ab
//        {
//#if Linux
//            Console.WriteLine("exited?" + matlab_process.HasExited);
//           // matlab_process.Kill(); // test ob das unter linux geht. Unter windows existiert der Startprozess nicht mehr. Unter Linux wirft es keine Fehlermeldung beendet den prozes aber auch nicht
//            Console.WriteLine("Process killed" + matlab_process.Id );
//            Console.WriteLine("exited?" + matlab_process.HasExited);
//# endif
//        }
    }
}
