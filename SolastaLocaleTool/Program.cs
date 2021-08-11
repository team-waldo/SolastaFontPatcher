using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityAssetLib;
using UnityAssetLib.Serialization;

namespace SolastaLocaleTool
{
    class Program
    {
        const string I2LANGUAGES_ASSET_NAME = "I2Languages";

        static void Main(string[] args)
        {
            if (args.Length == 1 && File.Exists(args[0]))
            {
                Export(args[0], "en.csv");
                return;
            }

            if (args.Length < 3)
            {
                PrintUsageAndExit();
            }

            string cmd = args[0];

            if (cmd == "export")
            {
                Export(args[1], args[2]);
            }
            else if (cmd == "import")
            {
                if (args.Length != 4)
                {
                    PrintUsageAndExit();
                }

                Import(args[1], args[2], args[3]);
            }
            else
            {
                Console.WriteLine($"Invalid command {cmd}");
                PrintUsageAndExit();
            }
        }

        static void Export(string assetPath, string csvPath, string langCode = "en")
        {
            var csvData = new List<string[]>();

            using (var af = AssetsFile.Open(assetPath))
            {
                var assetDef = af.GetAssetByName(I2LANGUAGES_ASSET_NAME, ClassIDType.MonoBehaviour);
                var serializer = new UnitySerializer(af);

                var i2lang = serializer.Deserialize<LanguageSourceAsset>(assetDef);
                var source = i2lang.mSource;

                var langIndex = source.mLanguages.FindIndex(x => x.Code == langCode);
                if (langIndex == -1)
                {
                    Console.WriteLine($"ERROR: Language code {langCode} does not exist.");
                    return;
                }

                for (int i = 0; i < source.mTerms.Count; i++)
                {
                    var termData = source.mTerms[i];

                    var key = termData.Term;
                    var eng = termData.Languages[langIndex];
                    
                    if (!string.IsNullOrWhiteSpace(eng) && !eng.StartsWith("[OBSOLETE]", StringComparison.InvariantCultureIgnoreCase))
                        csvData.Add(new string[] { key, eng });
                }
            }

            using (var file = File.Create(csvPath))
            using (var writer = new StreamWriter(file))
            {
                Csv.CsvWriter.Write(writer, new string[] { "Key", "Source", "Translation" }, csvData);
            }
        }

        static void Import(string assetPath, string csvPath, string assetOutputPath)
        {
            var translationData = new Dictionary<string, string>();

            using (var file = File.OpenRead(csvPath))
            using (var reader = new StreamReader(file))
            {
                var opt = new Csv.CsvOptions() {
                    AllowNewLineInEnclosedFieldValues = true,
                    AllowSingleQuoteToEncloseFieldValues = true
                };

                foreach (var row in Csv.CsvReader.Read(reader, opt))
                {
                    if (row.ColumnCount < 3)
                        continue;

                    string key = row[0];
                    string src = row[1];
                    string dst = row[2];

                    if (dst.Length > 0)
                        translationData.Add(key, dst);
                }
            }

            Console.WriteLine($"Loaded {translationData.Count} translations");

            using (var af = AssetsFile.Open(assetPath))
            {
                var assetDef = af.GetAssetByName(I2LANGUAGES_ASSET_NAME, ClassIDType.MonoBehaviour);
                var serializer = new UnitySerializer(af);

                var i2lang = serializer.Deserialize<LanguageSourceAsset>(assetDef);
                var source = i2lang.mSource;

                var engIndex = source.mLanguages.FindIndex(x => x.Code == "en");
                for (int i = 0; i < source.mTerms.Count; i++)
                {
                    var termData = source.mTerms[i];

                    var key = termData.Term;
                    if (translationData.TryGetValue(key, out string tr))
                    {
                        termData.Languages[engIndex] = tr;
                    }
                }

                af.ReplaceAsset(assetDef.pathID, serializer.Serialize(i2lang));

                af.Save(assetOutputPath);
            }
        }

        static void PrintUsageAndExit()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("SolastaLocaleTool.exe export resource.assets csv_output_path.csv");
            Console.WriteLine("SolastaLocaleTool.exe import resource.assets csv_input_path.csv output_resource.assets");
            Console.WriteLine("SolastaLocaleTool.exe resource.assets");
            Environment.Exit(1);
        }
    }
}
