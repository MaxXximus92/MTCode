using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class SSHTest
    {

        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hell");
        //    using (var ssh = new SshClient(CreateConnectionInfo()))
        //    {
        //        ssh.Connect();
    
        //        ShellStream stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);
        //        var result1= SendCommand(stream, "ls");
        //        var result2 = SendCommand(stream, "2+2");
        //        //var command = ssh.CreateCommand("ls");
        //        //var result = command.Execute();
        //        //var command2 = ssh.CreateCommand("2+2");
        //        //var result2 = command2.Execute();
        //        Console.Out.WriteLine(result1);
        //        Console.Out.WriteLine(result2);
        //        ssh.Disconnect();
        //    }
        //}

        public static ConnectionInfo CreateConnectionInfo()
        {
            ConnectionInfo connectionInfo;

            AuthenticationMethod authenticationMethod = new PasswordAuthenticationMethod("mutschler", "12345");


            connectionInfo = new ConnectionInfo(
                "eciton",
                "mutschler",
                authenticationMethod);
            // }

            return connectionInfo;
        }


        private static string SendCommand(ShellStream stream, string customCMD)
        {
            StringBuilder strAnswer = new StringBuilder();

            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            WriteStream(customCMD, writer, stream);

            strAnswer.AppendLine(ReadStream(reader));

            string answer = strAnswer.ToString();
            return answer.Trim();
        }

        private static void WriteStream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.WriteLine(cmd);
            while (stream.Length == 0)
                Thread.Sleep(500);
        }

        private static string ReadStream(StreamReader reader)
        {
            StringBuilder result = new StringBuilder();

            string line;
            while ((line = reader.ReadLine()) != null)
                result.AppendLine(line);

            return result.ToString();
        }
    }
}
