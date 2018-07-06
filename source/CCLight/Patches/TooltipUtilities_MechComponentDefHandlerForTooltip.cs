
using System;
using BattleTech;
using BattleTech.UI.Tooltips;
using Harmony;

namespace CustomComponents
{
    [HarmonyPatch(typeof(TooltipUtilities), "MechComponentDefHandlerForTooltip")]
    public static class TooltipUtilitiesMechComponentDefHandlerForTooltipPatch
    {
        public static bool Prefix(MechComponentDef baseDef, ref object __result)
        {
            __result = baseDef as ICustomComponent;
            return __result == null;
        }
    }
}
