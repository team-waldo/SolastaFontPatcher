using System;
using System.IO;

using UnityAssetLib;
using UnityAssetLib.Serialization;

namespace SolastaLocaleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsageAndExit();
            }

            string cmd = args[0];

            if (cmd == "export")
            {
                Export(args);
            }
            else if (cmd == "import")
            {
                Import(args);
            }
            else
            {
                Console.WriteLine($"Invalid command {cmd}");
                PrintUsageAndExit();
            }
        }

        static void Export(string[] args)
        {
            using (var af = AssetsFile.Open(args[1]))
            {
                af.GetAssetByName()
            }
        }

        static void Import(string[] args)
        {

        }

        static void PrintUsageAndExit()
        {
            Console.WriteLine("usage: SolastaLocaleTool.exe export resource.assets csv_path");
            Console.WriteLine("usage: SolastaLocaleTool.exe import resource.assets csv_path resource.assets.new");
            Environment.Exit(1);
        }


    }
}
