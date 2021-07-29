using System;
using System.IO;

using UnityAssetLib;
using UnityAssetLib.Serialization;

namespace SolastaLocaleTool
{
    class Program
    {
        const string I2LANGUAGES_ASSET_NAME = "I2Languages";

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
                var asset = af.GetAssetByName(I2LANGUAGES_ASSET_NAME);
            }
        }

        static void Import(string[] args)
        {
            using (var af = AssetsFile.Open(args[1]))
            {
                // TODO
            }
        }

        static void PrintUsageAndExit()
        {
            Console.WriteLine("usage: SolastaLocaleTool.exe export resource.assets csv_output_path.csv");
            Console.WriteLine("usage: SolastaLocaleTool.exe import resource.assets csv_input_path.csv output_resource.assets");
            Environment.Exit(1);
        }
    }
}
