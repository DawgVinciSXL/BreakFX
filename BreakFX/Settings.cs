using System;
using UnityEngine;
using UnityModManagerNet;

namespace BreakFX
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw(DrawType.KeyBinding)] public KeyBinding BreakKey = new KeyBinding { keyCode = KeyCode.B };

        public void OnChange()
        {
            throw new NotImplementedException();
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save<Settings>(this, modEntry);
        }
    }
}