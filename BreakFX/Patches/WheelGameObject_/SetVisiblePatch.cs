using HarmonyLib;

namespace BreakFX.Patches.WheelGear
{
    [HarmonyPatch(typeof(WheelGearObject), "SetVisible")]
    internal class SetVisiblePatch
    {
        private static void Postfix(WheelGearObject __instance)
        {
            if (Main.enabled)
            {
                //if (Logic.Instance.defaultWheelsFound == false)
                //{
                //    Logic.Instance.defaultWheelMesh = __instance.gameObjects[0].GetComponent<MeshFilter>().mesh;
                //    Logic.Instance.defaultWheelsFound = true;
                //}

                BreakFXController.Instance.wheelGameObjects = __instance.gameObjects;
            }
        }
    }
}