using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace BreakFX
{
    public static class Main
    {
        public static Harmony harmonyInstance;
        public static UnityModManager.ModEntry.ModLogger logger;
        public static Settings settings;
        public static UnityModManager.ModEntry modEntry;

        public static BreakFXController breakFXController;

        public static bool enabled;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
            Main.modEntry = modEntry;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value == enabled)
            {
                return true;
            }

            enabled = value;

            if (enabled)
            {
                harmonyInstance = new Harmony(modEntry.Info.Id);
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                AssetBundleHelper.LoadUIBundle();
                breakFXController = new GameObject().AddComponent<BreakFXController>();
                UnityEngine.Object.DontDestroyOnLoad(breakFXController);
            }
            else
            {
                harmonyInstance.UnpatchAll(harmonyInstance.Id);
                UnityEngine.Object.Destroy(breakFXController);
            }
            return true;
        }
    }
}