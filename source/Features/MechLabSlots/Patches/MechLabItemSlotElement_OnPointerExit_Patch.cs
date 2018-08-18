using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabItemSlotElement), "OnPointerExit")]
    public static class MechLabItemSlotElement_OnPointerExit_Patch
    {
        public static bool Prefix(MechLabItemSlotElement __instance)
        {
            return __instance.DropParent != null;
        }
    }
}