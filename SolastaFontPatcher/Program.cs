using System;
using System.IO;

using UnityAssetLib;
using UnityAssetLib.Serialization;

namespace SolastaFontPatcher
{
    public class Program
    {
        private const string GAME_DIR_NAME = "Slasta_COTM";
        private const string GAME_EXE_NAME = "Solasta.exe";
        private const string GAME_DATA_DIR_NAME = "Solasta_Data";

        private static readonly string[] defaultPaths = {
            @".",
            @"..",
            @"..\..",
        };

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("\n\t\t솔라스타 한글 폰트 패치\n\t\t\t\t\t@akintos\n");

            var gamePath = FindGame();

            if (gamePath is null)
            {
                Console.WriteLine("게임 설치 경로를 찾지 못했습니다. 패치를 종료합니다...");
                System.Threading.Thread.Sleep(2000);
                return;
            }

            Console.WriteLine("게임 설치 경로:\n" + gamePath);
            FontPatcher patcher = new FontPatcher(Path.Combine(gamePath, GAME_DATA_DIR_NAME));

            bool result = patcher.PatchFont();

            if (result)
                Console.WriteLine("\n패치가 완료되었습니다.");
            else
                Console.WriteLine("\n패치가 실패했습니다.");
            
            System.Threading.Thread.Sleep(2000);
            return;
        }

        private static string FindGame()
        {
            Console.WriteLine("게임 설치 경로 탐색중...\n");

            foreach (var possibleDir in defaultPaths)
            {
                if (File.Exists(Path.Combine(possibleDir, GAME_EXE_NAME)))
                {
                    return Path.GetFullPath(possibleDir);
                }
            }

            try
            {
                var steamPath = SteamFinder.FindSteamPath();
                if (steamPath != null)
                {
                    Console.WriteLine("스팀 설치 경로를 찾았습니다.");
                    string[] libraryPaths = SteamFinder.GetLibraryPaths(steamPath);

                    foreach (string libraryPath in libraryPaths)
                    {
                        var matches = Directory.GetDirectories(libraryPath, GAME_DIR_NAME);
                        if (matches.Length >= 1)
                            return matches[0];
                    }
                    Console.WriteLine("스팀 라이브러리에서 게임을 찾을 수 없습니다.\n");
                }
                else
                {
                    Console.WriteLine("스팀 설치 경로를 찾을 수 없습니다.\n");
                }
            }
            catch
            {

            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Title = $"게임 경로 수동 선택";
            dlg.FileName = "Solasta.exe"; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = $"{GAME_EXE_NAME} file|{GAME_EXE_NAME}"; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                Console.WriteLine("수동으로 게임 경로를 선택합니다.\n");
                return Path.GetDirectoryName(dlg.FileName);
            }
            else
            {
                Console.WriteLine("게임 경로 수동 선택을 취소했습니다.\n");
            }
            return null;
        }
    }
}
