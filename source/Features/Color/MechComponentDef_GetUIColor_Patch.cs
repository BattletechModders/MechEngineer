using System.Security.Permissions;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace CustomComponents
{
    [HarmonyPatch(typeof(MechComponentDef), "GetUIColor")]
    internal static class MechComponentDef_GetUIColor_Patch
    {

        [HarmonyPostfix]
        public static void Postfix(MechComponentDef componentDef, ref UIColor __result)
        {
            var comp = componentDef?.GetComponent<UIColorComponent>();
            if (comp == null)
            {
                return;
            }

            __result = comp.SlotElementUIColor;
        }
    }
}
