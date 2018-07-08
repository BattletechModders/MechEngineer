using BattleTech;
using BattleTech.UI;
using Harmony;

namespace CustomComponents
{
    [HarmonyPatch(typeof(MechComponentRef), "GetUIColor")]
    internal static class MechComponentRef_GetUIColor_Patch
    {

        [HarmonyPostfix]
        public static void Postfix(MechComponentRef componentRef, ref UIColor __result)
        {
            var comp = componentRef?.Def?.GetComponent<UIColorComponent>();
            if (comp == null)
            {
                return;
            }

            __result = comp.SlotElementUIColor;
        }
    }
}
