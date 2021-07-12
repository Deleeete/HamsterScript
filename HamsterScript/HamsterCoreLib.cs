using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hamster
{
    public static class HamsterCoreLib
    {
        /// <summary>
        /// Run the command interactively. All standard input/output/errors will be redirected
        /// </summary>
        /// <param name="cmd">command</param>
        public static void RunCommand(string cmd)
        {
            string[] args = cmd.Split();
            Process cmd_process = new();
            cmd_process.StartInfo.FileName = args[0]; ;
            cmd_process.StartInfo.Arguments = string.Join(" ", args, 1, args.Length - 1);;
            cmd_process.StartInfo.UseShellExecute = false;;
            cmd_process.StartInfo.RedirectStandardOutput = true;;
            cmd_process.StartInfo.RedirectStandardError = true;;
            cmd_process.StartInfo.RedirectStandardInput = true;;
            cmd_process.StartInfo.CreateNoWindow = true;;
            cmd_process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>{ Console.Error.WriteLine(e.Data); });
            cmd_process.OutputDataReceived += new DataReceivedEventHandler((sender, e) => { Console.Out.WriteLine(e.Data); });
            cmd_process.Start();
            using var stdin = cmd_process.StandardInput;

            var my_stdin = new StreamReader(Console.OpenStandardInput(), stdin.Encoding);
            Console.SetIn(my_stdin);
            Task.Run(() =>
            {
                char[] stdin_buffer = new char[65536];
                while (!cmd_process.HasExited)
                {
                    //my_stdin => stdin
                    int count = my_stdin.Read(stdin_buffer, 0, stdin_buffer.Length);
                    stdin.WriteAndClearBuffer(stdin_buffer, count);
                }
            });

            cmd_process.BeginErrorReadLine();
            cmd_process.BeginOutputReadLine();
            cmd_process.WaitForExit();
        }
        /// <summary>
        /// Run the command without interacting and obtain the command output string
        /// </summary>
        /// <param name="cmd">command</param>
        public static string GetCommandOutput(string cmd)
        {
            string output = "";
            string[] args = cmd.Split();
            Process cmd_process = new();
            cmd_process.StartInfo.FileName = args[0]; ;
            cmd_process.StartInfo.Arguments = string.Join(" ", args, 1, args.Length - 1); ;
            cmd_process.StartInfo.UseShellExecute = false; ;
            cmd_process.StartInfo.RedirectStandardOutput = true;
            cmd_process.StartInfo.RedirectStandardError = true;
            cmd_process.StartInfo.RedirectStandardInput = false;
            cmd_process.StartInfo.CreateNoWindow = true;
            cmd_process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => { output += $"{e.Data}\n"; });
            cmd_process.OutputDataReceived += new DataReceivedEventHandler((sender, e) => { output += $"{e.Data}\n"; });
            cmd_process.Start();
            cmd_process.BeginOutputReadLine();
            cmd_process.BeginErrorReadLine();
            cmd_process.WaitForExit();
            return output;
        }
    }

    static class StreamWriterExtension
    {
        public static void WriteAndClearBuffer(this StreamWriter sw, char[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                sw.Write(buffer[i]);
                buffer[i] = (char)0;
            }
        }
        public static void WriteAndClearBuffer(this StreamWriter sw, byte[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                sw.Write(buffer[i]);
                buffer[i] = 0;
            }
        }
    }
}
