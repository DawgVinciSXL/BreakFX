using System.IO;
using System.Reflection;
using UnityEngine;

namespace BreakFX
{
    public static class AssetBundleHelper
    {
        public static AudioClip BreakSFX { get; set; }
        public static GameObject BoardPrefab { get; set; }

        public static void LoadUIBundle()
        {
            var assetBundle = AssetBundle.LoadFromMemory(ExtractResources("BreakFX.Resources.boardutilsbbdata"));
            BoardPrefab = assetBundle.LoadAsset<GameObject>("assets/mods/boardutils/brokendeck.prefab");
            BreakSFX = assetBundle.LoadAsset<AudioClip>("assets/mods/boardutils/boardsnap.wav");
            assetBundle.Unload(false);
        }

        public static byte[] ExtractResources(string filename)
        {
            using (Stream resFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (resFileStream == null)
                {
                    return null;
                }
                else
                {
                    byte[] ba = new byte[resFileStream.Length];
                    resFileStream.Read(ba, 0, ba.Length);
                    return ba;
                }
            }
        }
    }
}