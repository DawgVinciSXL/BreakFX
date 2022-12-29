using HarmonyLib;

namespace BreakFX.Patches.TruckGearObject_
{
    [HarmonyPatch(typeof(TruckGearObject), "SetVisible")]
    class SetVisiblePatch
    {
        private static void Postfix(TruckGearObject __instance)
        {
            if (Main.enabled)
            {
                //if (__instance.gearInfo.type == "trucks")
                //{
                //    UIController.Instance.currentTruckType = "Generic";
                //}
                //else if (__instance.gearInfo.type == "trucksventure")
                //{
                //    UIController.Instance.currentTruckType = "Venture";
                //}
                //else if (__instance.gearInfo.type == "trucksthunder")
                //{
                //    UIController.Instance.currentTruckType = "Thunder";
                //}
                //else
                //{
                //    UIController.Instance.currentTruckType = "Independant";
                //}
                BreakFXController.Instance.truckGameObjects = __instance.gameObjects;
            }
        }
    }
}