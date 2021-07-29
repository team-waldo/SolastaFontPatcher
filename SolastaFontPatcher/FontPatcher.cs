using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using UnityAssetLib;
using UnityAssetLib.Serialization;
using UnityAssetLib.Types;

namespace SolastaFontPatcher
{
    public class FontPatcher
    {
        private const string SHAREDASSETS0_FILENAME = "sharedassets0.assets";

        public readonly string DataDirectoryPath;

        public FontPatcher(string dataDirectoryPath)
        {
            if (!Directory.Exists(dataDirectoryPath))
                throw new DirectoryNotFoundException("Cannot find Solasta_Data directory " + dataDirectoryPath);
            if (!File.Exists(Path.Combine(dataDirectoryPath, "resources.assets")))
                throw new ArgumentException($"{dataDirectoryPath} is not a valid Solasta data path");

            this.DataDirectoryPath = dataDirectoryPath;
        }

        public bool PatchFont()
        {
            string sharedassets0_path = GetDataFilePath(SHAREDASSETS0_FILENAME);
            string sharedassets0_backup = sharedassets0_path + ".bak";
            string sharedassets0_temp = sharedassets0_path + ".tmp";

            if (IsFileLocked(sharedassets0_path))
            {
                Console.WriteLine("sharedassets0.assets 파일이 사용 중입니다. 게임이 실행 중인지 확인하세요.");
                return false;
            }

            using (var sharedassets0 = AssetsFile.Open(sharedassets0_path))
            {
                if (sharedassets0.GetAssetByName("Noto-Bold SDF") == null)
                {
                    Console.WriteLine("패치할 폰트를 찾을 수 없습니다. 이미 패치된 파일일 수 있습니다.");
                    return false;
                }

                var boldPathId = ReplaceFont(sharedassets0, "Noto-Bold SDF", "Data/bold_font.dat", "Data/bold_texture.bin");
                var regularPathid = ReplaceFont(sharedassets0, "Noto-Regular SDF", "Data/regular_font.dat", "Data/regular_texture.bin");
                var lightPathid = ReplaceFont(sharedassets0, "Noto-Thin SDF", "Data/light_font.dat", "Data/light_texture.bin");

                AddFallbackFont(sharedassets0, "LiberationSans SDF", 0, regularPathid);

                sharedassets0.Save(sharedassets0_temp);
            }

            if (File.Exists(sharedassets0_backup))
                File.Delete(sharedassets0_backup); // Remove old backup

            File.Move(sharedassets0_path, sharedassets0_backup);
            File.Move(sharedassets0_temp, sharedassets0_path);

            return true;
        }

        private long ReplaceFont(AssetsFile af, string fontName, string newFontDef, string newFontTexture)
        {
            var fontAssetInfo = af.GetAssetByName(fontName);
            var serializer = new UnitySerializer(af);

            var oldFont = serializer.Deserialize<TMP_FontAsset>(fontAssetInfo);
            var newFont = serializer.Deserialize<TMP_FontAsset>(File.ReadAllBytes(newFontDef));

            var fontTexturePathId = oldFont.m_AtlasTextures[0].m_PathID;

            newFont.m_Script = oldFont.m_Script;
            newFont.material = oldFont.material;
            newFont.m_AtlasTextures = oldFont.m_AtlasTextures;

            var newFontBytes = serializer.Serialize(newFont);
            af.ReplaceAsset(fontAssetInfo.pathID, newFontBytes);

            // Write raw texture data to a file
            var fontTextureDataPath = GetDataFilePath(newFont.m_Name + " Texture.bin");
            File.Copy(newFontTexture, fontTextureDataPath, overwrite: true);

            // Update font texture info
            var texture = serializer.Deserialize<Texture2D>(af.assets[fontTexturePathId]);

            texture.m_Width = newFont.m_AtlasWidth;
            texture.m_Height = newFont.m_AtlasHeight;

            texture.imageData = new byte[0];

            texture.m_StreamData.offset = 0;
            texture.m_StreamData.size = (uint)(newFont.m_AtlasWidth * newFont.m_AtlasHeight);
            texture.m_StreamData.path = fontTextureDataPath;

            var textureBytes = serializer.Serialize(texture);
            af.ReplaceAsset(fontTexturePathId, textureBytes);

            return fontAssetInfo.pathID;
        }

        private void AddFallbackFont(AssetsFile af, string fontName, int fallbackFontFileId, long fallbackFontPathId)
        {
            var fontAssetInfo = af.GetAssetByName(fontName);

            var serializer = new UnitySerializer(af);
            var font = serializer.Deserialize<TMP_FontAsset>(fontAssetInfo);

            var fallback = font.m_FallbackFontAssetTable.ToList();
            fallback.Add(new PPtr() { m_FileID = fallbackFontFileId, m_PathID = fallbackFontPathId });
            font.m_FallbackFontAssetTable = fallback.ToArray();

            af.ReplaceAsset(fontAssetInfo.pathID, serializer.Serialize(font));
        }

        protected static bool IsFileLocked(string path)
        {
            try
            {
                using (FileStream stream = new FileInfo(path).Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    stream.Close();
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        private string GetDataFilePath(string filename)
        {
            return Path.Combine(DataDirectoryPath, filename);
        }
    }
}
