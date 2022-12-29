using HarmonyLib;

namespace BreakFX.Patches.DeckGear
{
    [HarmonyPatch(typeof(DeckGearObject), "SetVisible")]
    class SetVisiblePatch
    {
        private static void Postfix(DeckGearObject __instance)
        {
            if (Main.enabled)
            {
                BreakFXController.Instance.deckGameObject = __instance.gameObject;
            }
        }
    }
}