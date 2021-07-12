using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Gleee.Consoleee;

namespace Hamster
{
    class Program
    {
        public static readonly Consoleee console = new();
        static void Main(string[] args)
        {
            try
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                if (args.Length == 0)
                    console.WriteLn($"Hamster Script v{version.Major}.{version.Minor} Build {version.Build}-{version.Revision}");
                else
                {
                    string txt = "";
                    console.WriteOperationLn($"Reading file '{args[0]}'", () => 
                    {
                        txt = File.ReadAllText(args[0]);
                    });
                    string cs_code;
                    List<string> namespaces;
                    (cs_code, namespaces) = HamsterCompiler.TranslateToCSharp(args[0], txt);
                    File.WriteAllText(Path.GetFileNameWithoutExtension(args[0]) + ".cs", cs_code);
                    HamsterCompiler.ExecuteCSharp(cs_code, namespaces, args.TakeLast(args.Length - 1).ToArray());
                }
            }
            catch (Exception ex)
            {
                console.Write("\n");
                console.WriteLn($"{ex.Message}\nStack Trace: \n{ex.StackTrace}", ConsoleErrorLevel.Error);
            }
        }
    }
}
